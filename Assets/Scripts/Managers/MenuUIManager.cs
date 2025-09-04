using UnityEngine;

namespace Managers
{
    public class MenuUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject gameModeButtonsParent;
        [SerializeField] private GameObject dotModeButtonsParent;

        public void ProceedToDotModeSelection()
        {
            gameModeButtonsParent.SetActive(false);
            dotModeButtonsParent.SetActive(true);
        }
    }
}