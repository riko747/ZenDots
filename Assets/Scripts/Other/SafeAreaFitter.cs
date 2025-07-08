using UnityEngine;

namespace Other
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private const float Padding = 125f;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        private void ApplySafeArea()
        {
            var safeArea = Screen.safeArea;
            
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            
            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
            
            _rectTransform.offsetMin = new Vector2(Padding, Padding);
            _rectTransform.offsetMax = new Vector2(-Padding, -Padding);
        }
    }
}