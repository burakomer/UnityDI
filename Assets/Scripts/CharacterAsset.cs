using UnityEngine;

namespace DependencyInjection
{
    [CreateAssetMenu(fileName = "Character Asset", menuName = "Game/Character Asset", order = 1)]
    public class CharacterAsset : ScriptableObject
    {
        public string displayName;
    }
}