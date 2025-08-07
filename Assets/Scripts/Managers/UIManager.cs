using Interfaces.Managers;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour, IUIManager
    {
        [Header("Button Groups")]
        [SerializeField] private GameObject gameModeButtonsParent;
        [SerializeField] private GameObject dotModeButtonsParent;
        
        [Header("Screens")]
        [SerializeField] private GameObject retryScreen;

        public void ProceedToDotModeSelection()
        {
            gameModeButtonsParent.SetActive(false);
            dotModeButtonsParent.SetActive(true);
        }
        
        public void ShowRetryButton()
        {
            if (retryScreen != null)
            {
                retryScreen.SetActive(true);
            }
        }
    }
}
