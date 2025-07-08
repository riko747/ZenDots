using Buttons;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject retryScreen;
        
        public void ShowRetryButton()
        {
            retryScreen.SetActive(true);
        }
    }
}
