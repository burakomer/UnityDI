using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DwarfEngine.Tools.Pooling
{
    public interface IUnityObjectPool<T> : IObjectPool, IEnumerable<T> where T : Object
    {
        void Initialize(string containerName, Transform containerParent, Action<T> onAddToPool = null);
        T GetPooledObject(bool setActive);
        void ReturnToPool(T poolObject);
    }
}