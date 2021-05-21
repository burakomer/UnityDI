using System;
using System.Collections.Generic;

namespace DwarfEngine.DependencyKit
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AssetInjectMethodAttribute : Attribute
    {
        /// <summary>
        /// Used to define ordering of parameters, regardless of their position in the file.
        /// </summary>
        public readonly List<string> refNames;

        /// <summary>
        /// Attribute for the asset injection method. Takes in the names of the reference fields.
        /// </summary>
        public AssetInjectMethodAttribute(params string[] refNames)
        {
            this.refNames = new List<string>(refNames);
            // this.refNames = new List<string>(refNames.Length);
            // foreach (var t in refNames)
            //     this.refNames.Add(t);
        }
    }
}