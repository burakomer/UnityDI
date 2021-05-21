using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DwarfEngine.Tools.Pooling
{
    /// <summary>
    /// An object pool for <see cref="UnityEngine.Object"/> objects.
    /// Usually used with <see cref="GameObject"/> and <see cref="Component"/> types.
    /// </summary>
    /// <typeparam name="T">Type of the pooled object.</typeparam>
    public class UnityObjectPool<T> : IUnityObjectPool<T> where T : Object
    {
        public event Action<T> OnAddToPool;

        private T _objectToPool;
        private int _amountToPool;
        private bool _expandInNeed;
        private int _expandAmount = 1;

        private List<T> _pooledObjects;
        private Transform _container;

        public UnityObjectPool()
        {
        }
        
        public UnityObjectPool(T objectToPool, int amountToPool, bool expandInNeed, int expandAmount = 1)
        {
            _objectToPool = objectToPool;
            _amountToPool = amountToPool;
            _expandInNeed = expandInNeed;
            _expandAmount = expandAmount;
        }

        public void Initialize(string containerName, Transform containerParent, Action<T> onAddToPool = null)
        {
            if (onAddToPool != null) OnAddToPool += onAddToPool;
            
            if (!string.IsNullOrEmpty(containerName))
            {
                _container = new GameObject($"{containerName} Pool").transform;

                if (containerParent != null) _container.transform.SetParent(containerParent);
            }
            else
            {
                // For UI pools, container must be created already. Parent IS the container.
                _container = containerParent;
            }

            _pooledObjects = new List<T>();
            for (int i = 0; i < _amountToPool; i++)
                AddToPool();

            Initialized = true;
        }

        public T GetPooledObject(bool setActive)
        {
            T returnObj;
            for (int i = 0; i < _pooledObjects.Count; i++)
            {
                returnObj = _pooledObjects[i];
                if (IsActiveInHierarchy(returnObj)) continue;

                GameObjectOf(returnObj).SetActive(setActive);
                return returnObj;
            }

            if (!_expandInNeed) return null;

            returnObj = AddToPool();
            for (int i = 0; i < _expandAmount - 1; i++) AddToPool();
            OnAddToPool?.Invoke(returnObj);

            GameObjectOf(returnObj).SetActive(setActive);
            return returnObj;
        }

        public void ReturnToPool(T poolObject)
        {
            var go = GameObjectOf(poolObject);
            go.transform.SetParent(_container.transform);
            go.SetActive(false);
        }

        private T AddToPool()
        {
            var returnObj = Object.Instantiate(_objectToPool, _container.transform, true);
            GameObjectOf(returnObj).SetActive(false);
            _pooledObjects.Add(returnObj);
            OnAddToPool?.Invoke(returnObj);
            return returnObj;
        }

        /// <summary>
        /// Call this in OnDestroy of MonoBehaviour.
        /// </summary>
        public void Destroy()
        {
            if (_container != null) Object.Destroy(_container.gameObject);
        }

        public void ReturnChildrenToPool(Transform transform)
        {
            if (transform.childCount == 0) return;
            foreach (Transform poolObject in transform)
            {
                poolObject.SetParent(_container.transform);
                poolObject.gameObject.SetActive(false);
            }
        }
        
        public IEnumerator<T> GetEnumerator() => _pooledObjects.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Initialized { get; set; }

        public void Initialize(object objectToPool, int amountToPool, bool expandInNeed, int expandAmount = 1)
        {
            _objectToPool = (T) objectToPool;
            _amountToPool = amountToPool;
            _expandInNeed = expandInNeed;
            _expandAmount = expandAmount;
            
            Initialize(_objectToPool.name, null);
        }

        public object GetPooledObject() => GetPooledObject(true);

        // HELPERS
        
        private bool IsActiveInHierarchy(T obj)
        {
            switch (obj)
            {
                case GameObject go:
                    return go.activeInHierarchy;
                case Component comp:
                    return comp.gameObject.activeInHierarchy;
                default:
                    return false;
            }
        }

        private GameObject GameObjectOf(T obj)
        {
            switch (obj)
            {
                case GameObject gameObject:
                    return gameObject;
                case Component component:
                    return component.gameObject;
                default:
                    return null;
            }
        } 
    }
}