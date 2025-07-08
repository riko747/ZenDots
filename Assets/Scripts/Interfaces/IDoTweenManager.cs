using Managers;
using UnityEngine;

namespace Interfaces
{
    public interface IDoTweenManager
    {
        public void PlayFadeAnimation(GameObject parent, DoTweenManager.FadeType fadeType);
        public void PlayPopAnimation(RectTransform transform);
    }
}