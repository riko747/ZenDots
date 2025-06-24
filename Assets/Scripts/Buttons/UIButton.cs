using UnityEngine;
using UnityEngine.UI;

namespace Buttons
{
    public abstract class UIButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        private void Start() => button.onClick.AddListener(HandleButton);

        protected abstract void HandleButton();

        private void OnDestroy() => button.onClick.RemoveListener(HandleButton);

    }
}
