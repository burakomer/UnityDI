using System.Collections.Generic;
using DwarfEngine.DependencyKit;
using DwarfEngine.Tools.Pooling;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DependencyInjection
{
    public class NpcCharacter : MonoBehaviour, IDependent, ICustomBinding
    {
        [Header("Addressables")]
        [SerializeField] private AssetReferenceItemAsset _itemAssetRef;
        [SerializeField] private AssetReference _characterAssetRef;
        [SerializeField] private AssetLabelReference _weaponsLabel;
        [SerializeField] private AssetReference _fooRef;

        private CharacterAsset CharacterAsset { get; set; }
        private ItemAsset ItemAsset { get; set; }
        private List<ItemAsset> Weapons { get; set; }

        private Weapon _weapon;

        private IUnityObjectPool<PooledFoo> FooPool { get; set; }

        private PooledFoo _currentFoo;
        private bool _initialized;

        public void LoadBindings()
        {
            this.Bind<Weapon>().To<Bow>();
        }
        
        [ServiceInjectMethod]
        private void ServiceDependencies(
            GameManager gameManager,
            AnotherManager anotherManager)
        {
        }
        
        [AssetInjectMethod(
            nameof(_characterAssetRef), 
            nameof(_itemAssetRef), 
            nameof(_weaponsLabel), 
            nameof(_fooRef),
            nameof(_weapon))]
        private void AssetDependencies(
            CharacterAsset characterAsset, 
            [AssetInject] ItemAsset itemAsset, 
            [AssetInject(AssetInjectType.Group)] List<ItemAsset> weapons,
            [AssetInject(AssetInjectType.Pool)] IUnityObjectPool<PooledFoo> fooPool)
        {
            CharacterAsset = characterAsset;
            ItemAsset = itemAsset;
            Weapons = weapons;
            FooPool = fooPool;

            _initialized = true;
        }

        private float _timer = 2f;
        private float _downTimer = 1f;

        private void Update()
        {
            if (!_initialized) return;
            
            if (_downTimer <= 0)
            {
                if (_timer <= 0 && _currentFoo != null)
                {
                    FooPool.ReturnToPool(_currentFoo);
                    _currentFoo = null;
                    _downTimer = 1f;
                    return;
                }

                if (_timer > 0 && _currentFoo == null)
                {
                    _currentFoo = FooPool.GetPooledObject(true);
                    _currentFoo.Initialize(this);
                    return;
                }

                _timer -= Time.deltaTime;
            }
            else
            {
                _downTimer -= Time.deltaTime;
                if (_downTimer <= 0) _timer = 2f;
            }
        }
    }
}