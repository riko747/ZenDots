using Interfaces;
using Other;
using UnityEngine;
using Zenject;

namespace Core
{
    public class DotSpawner : MonoBehaviour
    {
        [Inject] private IGameManager _gameManager;

        [SerializeField] private RectTransform gameArea;
        
        private ISpawnStrategy _currentDotSpawner;

        private void Start()
        {
            if (_gameManager.GetGameMode() == Constants.GameMode.Default)
            {
                _currentDotSpawner = _gameManager.Instantiator.Instantiate<DefaultModeDotSpawner>
                (
                    new object[] { gameArea }
                );
            }
            else if (_gameManager.GetGameMode() == Constants.GameMode.Zen)
            {
                
            }
            _currentDotSpawner.Spawn();
        }
    }
}
