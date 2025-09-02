using Interfaces.Managers;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class Initializer : MonoBehaviour
    {
        [Inject] private IGameManager _gameManager;
        [Inject] private ILevelManager _levelManager;

        private void Awake()
        {
            _gameManager.Initialize();
            _levelManager.Initialize();
        }
    }
}
