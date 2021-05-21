using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DwarfEngine.Tools;
using DwarfEngine.Tools.Pooling;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace DwarfEngine.DependencyKit
{
    internal class DKManagerInternal
    {
        internal DKBindingsInternal bindings;

        private readonly Dictionary<Type, MonoBehaviour> _gameServices;
        private readonly Dictionary<Type, MonoBehaviour> _sceneServices;
        private readonly Dictionary<Type, IObjectPool> _globalPools;

        private readonly DKManager _component;
        
        public DKManagerInternal(DKManager component)
        {
            _component = component;
            
            _gameServices = new Dictionary<Type, MonoBehaviour>();
            _sceneServices = new Dictionary<Type, MonoBehaviour>();
            _globalPools = new Dictionary<Type, IObjectPool>();
        }
        
        public void GatherAndInject()
        {
            Debug.Log("Gathering and injecting dependencies...");
            var dependents = DEUtils.GameObjects.FindComponents<IDependent>();
            GatherDependencies(in dependents);
            InjectDependencies(in dependents);
        }

        public void Release()
        {
            _sceneServices.Clear();
            _globalPools.Clear();
        }

        public void InjectDependencies(in List<IDependent> dependents)
        {
            foreach (var dependent in dependents)
            {
                var dependentType = dependent.GetType();
                
                var dependentMethods = dependentType
                    .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

                MethodInfo serviceInjectMethod = null;
                MethodInfo assetInjectMethod = null;

                foreach (var method in dependentMethods)
                {
                    if (Attribute.IsDefined(method, typeof(ServiceInjectMethodAttribute))) 
                        serviceInjectMethod = method;
                    else if (Attribute.IsDefined(method, typeof(AssetInjectMethodAttribute))) 
                        assetInjectMethod = method;
                }

                // Inject 
                if (serviceInjectMethod != null) InjectServices(dependent, in serviceInjectMethod);
                if (assetInjectMethod != null) _component.StartCoroutine(InjectAssets(dependent, dependentType, assetInjectMethod));
            }
        }

        /// <summary>
        /// Get all dependencies and register them.
        /// </summary>
        private void GatherDependencies(in List<IDependent> dependents)
        {
            using (var finder = DEFinder.GetFinder())
            {
                foreach (var gameService in finder.FindComponents<IGameService>())
                {
                    var service = RegisterService(gameService as MonoBehaviour, in _gameServices);
                    if (service != null) Object.DontDestroyOnLoad(service);
                }

                foreach (var sceneService in finder.FindComponents<ISceneService>())
                {
                    RegisterService(sceneService as MonoBehaviour, in _sceneServices);
                }

                foreach (var customBinding in finder.FindComponents<ICustomBinding>())
                {
                    customBinding.LoadBindings();
                }
            }
        }

        private MonoBehaviour RegisterService(
            MonoBehaviour service,
            in Dictionary<Type, MonoBehaviour> serviceDict)
        {
            var objectType = service.GetType();
            if (_gameServices.ContainsKey(objectType))
            {
                Debug.LogWarning($"Service named {objectType.Name} already exists in the scene! Destroying extra.");
                Object.Destroy(service.gameObject);
                return null;
            }
            serviceDict.Add(objectType, service);
            return service;
        }
        
        
        #region Service Injection

        private void InjectServices(object dependent, in MethodInfo serviceInjectMethod)
        {
            var parameters = serviceInjectMethod.GetParameters();
            var dependencies = new object[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var dependencyType = parameter.ParameterType;
                dependencies[i] = GetService(dependencyType);
            }

            serviceInjectMethod.Invoke(dependent, dependencies);
        }

        private object GetService(Type dependencyType)
        {
            var interfaces = dependencyType.GetInterfaces().ToList();
            
            if (interfaces.Contains(typeof(IGameService)))
            {
                if (_gameServices.TryGetValue(dependencyType, out var service) == false)
                    throw new NullReferenceException($"GameService for type {dependencyType.Name} is not registered!");
                return service;
            }

            if (interfaces.Contains(typeof(ISceneService)))
            {
                if (_sceneServices.TryGetValue(dependencyType, out var service) == false)
                    throw new NullReferenceException($"SceneService for type {dependencyType.Name} is not registered!");
                return service;
            }
            throw new NotImplementedException($"Type {dependencyType.Name} is not implemented in DependencyManager");
        }

        #endregion

        #region Asset Injection
        
        private IEnumerator InjectAssets(IDependent dependent, IReflect dependentType, MethodBase assetInjectMethod)
        {
            var refNames = assetInjectMethod.GetCustomAttribute<AssetInjectMethodAttribute>().refNames;
            var parameters = assetInjectMethod.GetParameters();
            var assetRefs = dependentType
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(field => typeof(AssetReference).IsAssignableFrom(field.FieldType) || 
                                typeof(AssetLabelReference).IsAssignableFrom(field.FieldType))
                .ToArray();

            var dependencyCount = refNames.Count;

            var loadingOpStatuses = new List<Func<AsyncOperationStatus>>(dependencyCount);
            var dependencies = new object[dependencyCount];
            
            // Start loading operations.
            foreach (var assetRef in assetRefs)
            {
                var paramIndex = refNames.IndexOf(assetRef.Name);
                var param = parameters[paramIndex];
                var fieldValue = assetRef.GetValue(dependent);

                var attribute = param.GetCustomAttribute<AssetInjectAttribute>();
                Func<AsyncOperationStatus> opStatusGetter;
                AsyncOperationHandle asyncOp;

                // Start operation for the asset loading.
                switch (attribute?.injectType)
                {
                    default:
                    case AssetInjectType.Instance:
                    {
                        asyncOp = GetAsset(fieldValue as AssetReference,
                            asset => dependencies[paramIndex] = asset);
                        opStatusGetter = () => asyncOp.Status;
                        break;
                    }
                    case AssetInjectType.Group:
                    {
                        dependencies[paramIndex] = (IList) MakeGenericObjectFromParameter(typeof(List<>), param);
                        asyncOp = GetAssets(fieldValue as AssetLabelReference,
                            asset => ((IList) dependencies[paramIndex]).Add(asset));
                        opStatusGetter = () => asyncOp.Status;
                        break;
                    }
                    case AssetInjectType.Pool:
                    {
                        var assetType = param.ParameterType.GetGenericArguments()[0];
                        var poolAlreadyExists = _globalPools.ContainsKey(assetType);
                        if (poolAlreadyExists)
                        {
                            var pool = _globalPools[assetType];
                            dependencies[paramIndex] = _globalPools[assetType];
                            opStatusGetter = () => pool.Initialized 
                                ? AsyncOperationStatus.Succeeded 
                                : AsyncOperationStatus.None;
                        }
                        else
                        {
                            var pool = (IObjectPool) MakeGenericObjectFromParameter(typeof(UnityObjectPool<>), param);
                            _globalPools.Add(assetType, pool);
                            dependencies[paramIndex] = pool;
                            asyncOp = GetAsset(
                                fieldValue as AssetReference,
                                asset =>
                                {
                                    var component = ((GameObject) asset).GetComponent(assetType);
                                    var poolSettings = assetType.GetCustomAttribute<ObjectPoolSettingsAttribute>();

                                    var amountToPool = poolSettings?.AmountToPool ?? 50;
                                    var expandInNeed = poolSettings?.ExpandInNeed ?? true;
                                    var expandAmount = poolSettings?.ExpandAmount ?? 50;
                                    _globalPools[assetType].Initialize(component, amountToPool, expandInNeed, expandAmount);
                                });
                            
                            opStatusGetter = () => asyncOp.Status;
                        }
                        break;
                    }
                }
                
                loadingOpStatuses.Add(opStatusGetter);
            }

            // Wait for the operations to complete.
            while (loadingOpStatuses.Count > 0)
            {
                for (var i = loadingOpStatuses.Count - 1; i >= 0; i--)
                {
                    var opStatus = loadingOpStatuses[i];
                    if (opStatus() == AsyncOperationStatus.None) continue;

                    loadingOpStatuses.RemoveAt(i);
                }
                yield return null;
            }
            
            // Inject the assets.
            assetInjectMethod.Invoke(dependent, dependencies);
        }

        private AsyncOperationHandle GetAsset(AssetReference assetRef, Action<Object> onComplete)
        {
            var asyncOp = assetRef.LoadAssetAsync<Object>();
            asyncOp.Completed += handle => onComplete(handle.Result);
            return asyncOp;
        }

        private AsyncOperationHandle GetAssets(AssetLabelReference labelRef, Action<Object> onComplete) => 
            Addressables.LoadAssetsAsync(labelRef.labelString, onComplete);

        private object MakeGenericObjectFromParameter(Type type, ParameterInfo param)
        {
            var paramGeneric = param.ParameterType.GenericTypeArguments[0];
            var genericType = type.MakeGenericType(paramGeneric);
            return Activator.CreateInstance(genericType);
        }

        #endregion
    }
}