using UnityEngine;
using UnityEngine.UI;

namespace Entities.Dot
{
    public class DotRipple : MonoBehaviour
    {
        [SerializeField] private Image rippleImage;
        
        public Image GetImage() => rippleImage;
        
        public void SetPosition(Vector2 position) => transform.position =  position;
        public void SetLocalScale(Vector3 scale) => rippleImage.transform.localScale = scale;
    }
}
