using UnityEngine;
using UnityEngine.UI;

namespace Buttons.DefaultMode
{
    public abstract class UIButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        protected virtual void Start() => button.onClick.AddListener(HandleButton);

        protected abstract void HandleButton();

        protected virtual void OnDestroy() => button.onClick.RemoveListener(HandleButton);

    }
}
