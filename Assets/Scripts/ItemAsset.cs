using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DependencyInjection
{
    [CreateAssetMenu(fileName = "Item Asset", menuName = "Game/Item Asset")]
    public class ItemAsset : ScriptableObject
    {
        public string displayName;
        public int price;
    }

    [Serializable]
    public class AssetReferenceItemAsset : AssetReferenceT<ItemAsset>
    {
        public AssetReferenceItemAsset(string guid) : base(guid)
        {
        }
    }
}