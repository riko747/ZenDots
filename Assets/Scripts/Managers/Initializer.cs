using UnityEngine;
using Zenject;

namespace Managers
{
    public class Initializer : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        [Inject] private LevelManager _levelManager;

        private void Awake()
        {
            _gameManager.Initialize();
            _levelManager.Initialize();
        }
    }
}
