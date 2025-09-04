using Interfaces.Managers;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [Inject] IGameManager gameManager;
        
        [SerializeField] private GameObject retryScreen;
        
        [SerializeField] private GameObject timer;

        private void Awake()
        {
            gameManager.OnLevelFailed += ShowRetryButton;
            gameManager.OnDotsSpawned += ShowTimer;
        }
        
        public void ShowTimer() => timer.SetActive(true);

        public void ShowRetryButton()
        {
            retryScreen.SetActive(true);
        }

        private void OnDestroy()
        {
            gameManager.OnLevelFailed -= ShowRetryButton;
            gameManager.OnDotsSpawned -= ShowTimer;
        }
    }
}
