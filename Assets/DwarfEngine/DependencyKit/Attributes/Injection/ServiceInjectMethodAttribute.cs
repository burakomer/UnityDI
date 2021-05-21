using System;

namespace DwarfEngine.DependencyKit
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ServiceInjectMethodAttribute : Attribute
    {
    }
}