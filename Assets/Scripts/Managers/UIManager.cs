using Buttons;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject retryButton;

        public void ShowUIButton()
        {
            retryButton.SetActive(true);
        }
    }
}
