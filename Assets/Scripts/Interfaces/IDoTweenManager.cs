using Entities.Dot;
using Managers;
using UnityEngine;

namespace Interfaces
{
    public interface IDoTweenManager
    {
        public void PlayFadeAnimation(GameObject parent, Dot dot, DoTweenManager.FadeType fadeType);
        public void PlayPopOutAnimation(RectTransform transform, Dot dot);
        public void PlayPopInAnimation(RectTransform transform, Dot dot);
        public void PlayRippleAnimation(Dot dot, DotRipple ripple);
    }
}