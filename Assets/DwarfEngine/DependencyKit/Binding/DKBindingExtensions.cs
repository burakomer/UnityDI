using UnityEngine;

namespace DwarfEngine.DependencyKit
{
    public static class DKBindingExtensions
    {
        /// <summary>
        /// Binds a <see cref="MonoBehaviour"/> type to a specific type.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <typeparam name="T">Source type of the binding.</typeparam>
        /// <returns>Returns the binding source instance to finalize binding.</returns>
        public static IBindingSource Bind<T>(this MonoBehaviour component)
        {
            var source = new BindingSource<T>(component.GetType());
            return source;
        }
    }
}