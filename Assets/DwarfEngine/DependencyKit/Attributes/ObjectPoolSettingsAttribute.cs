using System;

namespace DwarfEngine.DependencyKit
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ObjectPoolSettingsAttribute : Attribute
    {
        public int AmountToPool { get; }
        public bool ExpandInNeed { get; }
        public int ExpandAmount { get; }

        public ObjectPoolSettingsAttribute(int amountToPool = 50, bool expandInNeed = true, int expandAmount = 50)
        {
            AmountToPool = amountToPool;
            ExpandInNeed = expandInNeed;
            ExpandAmount = expandAmount;
        }
    }
}