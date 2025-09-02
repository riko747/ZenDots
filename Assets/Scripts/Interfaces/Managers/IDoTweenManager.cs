using System;
using Entities.Dot;
using UnityEngine;

namespace Interfaces.Managers
{
    public interface IDoTweenManager
    {
        public void PlayPopOutAnimation(RectTransform transform, Dot dot, Action callback =  null);
        public void PlayPopInAnimation(RectTransform transform, Dot dot , Action callback =  null);
        public void PlayRippleAnimation(Dot dot, DotRipple ripple, Action callback =  null);
    }
}