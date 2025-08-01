using Buttons.DefaultMode;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Entities.Dot
{
    public class Dot : MonoBehaviour
    {
        [Inject] private DoTweenManager _doTweenManager;
    
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Image image;
        [SerializeField] private DotButton button;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private DotRipple ripple;

        #region Getters
        public DotButton GetDotButton() => button;
        public DotRipple GetRipple() => ripple;
        public AudioSource GetAudioSource() => audioSource;
        public Vector2 GetPosition() => transform.position;
        public RectTransform GetTransform() => transform as RectTransform;
        public float GetDotSizeInPixelsX() => GetTransform().sizeDelta.x;
        public bool IsLast { get; private set; }
        public bool IsDeactivated { get; private set; }
        public bool IsPending { get; private set; }
        public float GetSizeInWorldSpace()
        {
            var corners = new Vector3[4];
            GetTransform().GetWorldCorners(corners);
            return Vector3.Distance(corners[0], corners[1]);
        }
        #endregion

        #region Setters
        public void SetPosition(Vector2 position) => transform.position = position;
        public void SetSize(float size) => GetTransform().sizeDelta = new Vector2(size, size);
        public void SetNumber(int number) => DotNumber = number;
        public void SetText(string text) => label.text = text;
        public void SetLast(bool last) => IsLast = last;
        public void DisableGraphics() => image.enabled = false;
        public void EnableGraphics() => image.enabled = true;
        
        public void SetActivatedState(bool activated)
        {
            if (activated)
            {
                IsDeactivated = false;
            }
            else
            {
                IsDeactivated = true;
                
            }
        }

        public void SetPendingState(bool pending)
        {
            if (pending)
            {
                IsPending = true;
            }
            else
            {
                IsPending = false;
            }
        }

        public void MoveUnderOtherDots() => transform.SetSiblingIndex(0);

        #endregion

        #region Properties
        public int DotNumber { get; private set; }
        #endregion
    
    }
}
