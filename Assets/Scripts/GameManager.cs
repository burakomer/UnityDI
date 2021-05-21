using System;
using DwarfEngine.DependencyKit;
using UnityEngine;

namespace DependencyInjection
{
    public class GameManager : MonoBehaviour, IGameService
    {
        public void StartGame(UnityEngine.Object caller)
        {
            Debug.Log("Game started!", caller);
        }
    }
}