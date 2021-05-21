using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace DwarfEngine.DependencyKit
{
    [DefaultExecutionOrder(-999)]
    public class DKManager : MonoBehaviour, IGameService
    {
        internal static DKManagerInternal _Manager;

        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 300, 100), "Reload Scene"))
            {
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            }
        }

        private void Awake()
        {
            // Gather and inject when a new scene loads.
            if (_Manager != null) { _Manager.GatherAndInject(); Destroy(gameObject); return; }
            
            // Initialize the manager.
            // Then, gather and inject dependencies for the first time.
            _Manager = new DKManagerInternal(this);
            _Manager.GatherAndInject();

            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.sceneLoaded += OnNewSceneLoaded;
        }

        /// <summary>
        /// Release scene-specific dependency references.
        /// </summary>
        private void OnSceneUnloaded(Scene scene)
        {
            Debug.LogWarning("Scene unloaded!");
            _Manager.Release();
        }

        /// <summary>
        /// Unload unused assets from the previous scene.
        /// </summary>
        private void OnNewSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.LogWarning("Scene loaded!");
            Resources.UnloadUnusedAssets();
        }
    }
    
    public static class DKFactory
    {
        public static GameObject CreateInstance(GameObject original)
        {
            var newInstance = Object.Instantiate(original);

            var dependents = new List<IDependent>();
            newInstance.GetComponentsInChildren(dependents);
            DKManager._Manager.InjectDependencies(dependents);

            return newInstance;
        }

        public static T CreateInstance<T>(GameObject original) =>
            CreateInstance(original).GetComponent<T>();

        public static Component CreateInstance(GameObject original, Type componentType) =>
            CreateInstance(original).GetComponent(componentType);
    }
}