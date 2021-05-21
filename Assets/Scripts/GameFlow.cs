using DwarfEngine.DependencyKit;
using UnityEngine;

namespace DependencyInjection
{
    public class GameFlow : MonoBehaviour, IDependent
    {
        private GameManager _gameManager;
        private DKManager _provider;

        [ServiceInjectMethod]
        private void Inject(GameManager gameManager, DKManager _provider, AnotherManager a)
        {
            _gameManager = gameManager;
            this._provider = _provider;
        }
        
        private void Awake()
        {
            _gameManager.StartGame(this);
        }
    }
}