using System;
using DG.Tweening;
using Entities.Dot;
using Interfaces.Managers;
using Other;
using UnityEngine;

namespace Managers
{
    public class DoTweenManager : IDoTweenManager
    {
        private Sequence _doTweenSequence;
        private Sequence _doTweenLoopSequence;

        public void PlayPopOutAnimation(RectTransform transform, Dot dot, Action callback = null)
        {
            KillIdleTween(dot.DotNumber);
            _doTweenSequence = DOTween.Sequence();
            _doTweenSequence.Append(transform.DOScale(1.2f, 0.3f));
            _doTweenSequence.Append(transform.DOScale(0.9f, 0.3f));
            _doTweenSequence.Join(transform.DOScale(0, 0.5f));
            _doTweenSequence.SetEase(Ease.OutBack);
            _doTweenSequence.Play().OnComplete(() => callback?.Invoke());
        }

        public void PlayPopInAnimation(RectTransform transform, Dot dot, Action callback = null, Action<Dot> callbackGeneric = null)
        {
            KillIdleTween(dot.DotNumber);
            _doTweenSequence = DOTween.Sequence();
            _doTweenSequence.Append(transform.DOScale(0.9f, 0.5f));
            _doTweenSequence.Append(transform.DOScale(1.2f, 0.3f));
            _doTweenSequence.Append(transform.DOScale(1, 0.3f));
            _doTweenSequence.SetEase(Ease.OutBack);
            _doTweenSequence.Play().OnComplete(() => callback?.Invoke());
        }

        public void PlayRippleAnimation(Dot dot, DotRipple ripple, Action callback = null)
        {
            _doTweenSequence = DOTween.Sequence();
            ripple.gameObject.SetActive(true);
            ripple.SetPosition(dot.GetPosition());
            var rippleImage = ripple.GetImage();
            rippleImage.color = new Color(1, 1, 1, 0.4f);
            ripple.SetLocalScale(Vector3.zero);
            _doTweenSequence.Join(dot.GetRipple().transform.DOScale(1.5f, 0.6f).SetEase(Ease.OutQuad));
            _doTweenSequence.Join(rippleImage.DOFade(0, 0.6f));
            _doTweenSequence.Play().OnComplete(() => ripple.gameObject.SetActive(false));
            
        }

        public void PlayIdleAnimation(Dot dot, int animationId, Action callback = null)
        {
            var startTimeDelay = UnityEngine.Random.Range(0.1f, 2f);
            var animationDuration = UnityEngine.Random.Range(1f, 3f);
            
            _doTweenLoopSequence = DOTween.Sequence();
            _doTweenLoopSequence.SetDelay(startTimeDelay)
                .Append(dot.GetTransform().DOScale(Constants.IdleAnimationScaleMax, animationDuration).SetEase(Ease.InOutSine))
                .Append(dot.GetTransform().DOScale(Constants.IdleAnimationScaleMin, animationDuration).SetEase(Ease.InOutSine))
                .Append(dot.GetTransform().DOScale(Constants.IdleAnimationScaleMax, animationDuration).SetEase(Ease.InOutSine))
                .SetLoops(-1, LoopType.Yoyo)
                .SetId("idle" + animationId)
                .SetAutoKill(false)
                .Play();;
        }

        public void ResetAnimations(Dot dot)
        {
            var dotTransform = dot?.GetTransform();
            if (!dotTransform) return;

            DOTween.Kill(dotTransform, complete: false);
            DOTween.Kill(dot, complete: false);
            dotTransform.localScale = Vector3.one;
            dotTransform.anchoredPosition3D = new Vector3(dotTransform.anchoredPosition3D.x, dotTransform.anchoredPosition3D.y, 0f);
        }

        public void KillIdleTween(int animationId)
        {
            DOTween.Kill("idle" + animationId);  
        }
    }
}