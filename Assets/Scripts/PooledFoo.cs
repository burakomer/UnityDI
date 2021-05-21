using System;
using DwarfEngine.DependencyKit;
using UnityEngine;

namespace DependencyInjection
{
    [ObjectPoolSettings(20)]
    public class PooledFoo : MonoBehaviour
    {
        private NpcCharacter _npcCharacter;

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Initialize(NpcCharacter npcCharacter)
        {
            _npcCharacter = npcCharacter;
        }

        private void OnEnable()
        {
            _rb.MovePosition(Vector2.zero);
        }

        private void OnDisable()
        {
            _npcCharacter = null;
        }
    }
}