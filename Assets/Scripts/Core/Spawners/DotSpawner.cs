using Interfaces.Managers;
using Interfaces.Strategies;
using Other;
using UnityEngine;
using Zenject;

namespace Core.Spawners
{
    public class DotSpawner : MonoBehaviour
    {
        [Inject] private IGameManager _gameManager;

        [SerializeField] private RectTransform gameArea;
        
        private ISpawnStrategy _currentDotSpawner;

        private void Start()
        {
            if (_gameManager.GetGameMode() == Constants.DefaultGameMode)
            {
                _currentDotSpawner = _gameManager.Instantiator.Instantiate<DefaultModeDotSpawner>
                (
                    new object[] { gameArea }
                );
            }
            else if (_gameManager.GetGameMode() == Constants.ZenGameMode)
            {
                _currentDotSpawner = _gameManager.Instantiator.Instantiate<ZenModeDotSpawner>(
                    new object[] { gameArea }
                );
            }
            _currentDotSpawner.Spawn();
        }
    }
}
