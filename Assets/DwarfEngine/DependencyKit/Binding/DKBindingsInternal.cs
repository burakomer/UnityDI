using System;
using System.Collections.Generic;

namespace DwarfEngine.DependencyKit
{
    internal class DKBindingsInternal
    {
        private readonly Dictionary<Type, List<Binding>> bindings;

        public DKBindingsInternal()
        {
            bindings = new Dictionary<Type, List<Binding>>();
        }

        /// <summary>
        /// Adds a binding to the MonoBehaviour type.
        /// </summary>
        /// <param name="componentType">Type of the component</param>
        /// <param name="binding">Created binding.</param>
        public void Add(Type componentType, Binding binding)
        {
            if (bindings.TryGetValue(componentType, out var bindingList))
                bindingList.Add(binding);
            else
                bindings.Add(componentType, new List<Binding> {binding});
        }
    }
    
    public interface IBindingSource
    {
        void To<TInstance>() where TInstance : class, new();
    }
    
    internal class BindingSource<TSource> : IBindingSource
    {
        private readonly Type _componentType;
            
        public BindingSource(Type componentType)
        {
            _componentType = componentType;
        }

        public void To<TInstance>() where TInstance : class, new()
        {
            var binding = new Binding(typeof(TSource), typeof(TInstance));
            DKManager._Manager.bindings.Add(_componentType, binding);
        }
    }

    internal class Binding
    {
        public readonly Type sourceType;
        public readonly Type instanceType;
        
        public Binding(Type sourceType, Type instanceType)
        {
            this.sourceType = sourceType;
            this.instanceType = instanceType;
        }
    }
}