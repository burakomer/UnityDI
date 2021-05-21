using System;

namespace DwarfEngine.DependencyKit
{
    public enum AssetInjectType
    {
        Instance,
        Group,
        Pool,
        Binding
    }
    
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class AssetInjectAttribute : Attribute
    {
        public readonly AssetInjectType injectType;
        
        public AssetInjectAttribute(AssetInjectType injectType = AssetInjectType.Instance)
        {
            this.injectType = injectType;
        }
    }
}