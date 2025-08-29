using Interfaces.Managers;
using Interfaces.Strategies;
using Other;
using UnityEngine;
using Zenject;

namespace Core.Spawn.Spawners
{
    public class DotSpawner : MonoBehaviour
    {
        [Inject] private IGameManager _gameManager;
        [SerializeField] private RectTransform gameArea;

        private ISpawnStrategy _current;

        private void Start()
        {
            if (_gameManager.GetGameMode() == Constants.DefaultGameMode)
                _current = _gameManager.Instantiator.Instantiate<DefaultModeDotSpawner>(new object[] { gameArea });
            else
                _current = _gameManager.Instantiator.Instantiate<ZenModeDotSpawner>(new object[] { gameArea });

            _current.Spawn();
        }

        private void OnDestroy()
        {
            if (_current is ZenModeDotSpawner zen) zen.Stop();
        }
    }
}