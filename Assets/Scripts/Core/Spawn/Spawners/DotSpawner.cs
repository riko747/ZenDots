using Interfaces.Managers;
using UnityEngine;
using Zenject;

namespace Core.Spawn.Spawners
{
    public class DotSpawner : MonoBehaviour
    {
        [Inject] private IGameManager _gameManager;
        [SerializeField] private RectTransform gameArea;

        private UnifiedDotSpawner _current;

        private void Start()
        {
            var mode = _gameManager.GetGameMode();
            _current = _gameManager.Instantiator.Instantiate<UnifiedDotSpawner>(new object[] { gameArea, mode });
            _current.Spawn();
        }

        private void OnDisable() => _current?.Stop();

        private void OnDestroy() => _current?.Stop();
    }
}