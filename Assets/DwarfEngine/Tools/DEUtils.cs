using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DwarfEngine.Tools
{
    public static class DEUtils
    {
        public static class GameObjects
        {
            private static GameObject[] _gameObjects;

            public static List<T> FindComponents<T>()
            {
                var interfaces = new List<T>();

                foreach (var loadedScene in Scenes.GetLoadedScenes())
                foreach (var rootGameObject in loadedScene.GetRootGameObjects())
                    interfaces.AddRange(rootGameObject.GetComponentsInChildren<T>());

                return interfaces;
            }
        }

        public static class Scenes
        {
            public static Scene[] GetLoadedScenes()
            {
                var countLoaded = SceneManager.sceneCount;
                var loadedScenes = new Scene[countLoaded];
 
                for (var i = 0; i < countLoaded; i++)
                {
                    loadedScenes[i] = SceneManager.GetSceneAt(i);
                }

                return loadedScenes;
            }
        }

        public static class Vectors
        {
            /// <summary>
            /// Returns a vector with a random direction and magnitude with given parameters.
            /// </summary>
            /// <param name="minMagnitude">Minimum magnitude of the vector.</param>
            /// <param name="maxMagnitude">Maximum magnitude of the vector.</param>
            /// <returns>Randomized direction.</returns>
            public static Vector2 RandomDirection(float minMagnitude, float maxMagnitude)
            {
                return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized *
                       Random.Range(minMagnitude, maxMagnitude);
            }
        }
    }
}