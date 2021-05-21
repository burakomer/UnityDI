using UnityEngine;

namespace DependencyInjection
{
    public class Bow : Weapon
    {
        public override void Attack()
        {
            Debug.Log("Shooting an arrow =------->");
        }
    }
}