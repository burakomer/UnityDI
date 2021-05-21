using System;
using System.Collections.Generic;
using UnityEngine;

namespace DwarfEngine.Tools
{
    public class DEFinder : IDisposable
    {
        // Static Members
        
        private static DEFinder _currentFinder;
        
        public static DEFinder GetFinder()
        {
            if (_currentFinder != null) return _currentFinder;
            
            _currentFinder = new DEFinder();
            return _currentFinder;
        }
        
        // Instance Members
        
        private readonly List<GameObject> _gameObjects;

        /// <summary>
        /// Gets all the game objects in all open scenes.
        /// </summary>
        private DEFinder()
        {
            _gameObjects = new List<GameObject>();
            foreach (var loadedScene in DEUtils.Scenes.GetLoadedScenes())
                _gameObjects.AddRange(loadedScene.GetRootGameObjects());
        }
        
        public List<T> FindComponents<T>()
        {
            var interfaces = new List<T>();

            foreach (var rootGameObject in _gameObjects)
                interfaces.AddRange(rootGameObject.GetComponentsInChildren<T>());

            return interfaces;
        }

        public void Dispose()
        {
            _currentFinder = null;
        }
    }
}