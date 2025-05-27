using UnityEngine;
using Zenject;

namespace Managers
{
    public class Initializer : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;

        private void Start()
        {
            _gameManager.Initialize();
        }
    }
}
