using System;
using DG.Tweening;
using Entities.Dot;
using Interfaces.Managers;
using UnityEngine;
namespace Managers
{
    public class DoTweenManager : IDoTweenManager
    {
        private Sequence _doTweenSequence;

        public void PlayPopOutAnimation(RectTransform transform, Dot dot, Action callback = null)
        {
            _doTweenSequence = DOTween.Sequence();
            _doTweenSequence.Append(transform.DOScale(1.2f, 0.3f));
            _doTweenSequence.Append(transform.DOScale(0.9f, 0.3f));
            _doTweenSequence.Join(transform.DOScale(0, 0.5f));
            _doTweenSequence.SetEase(Ease.OutBack);
            _doTweenSequence.Play().OnComplete(() => callback?.Invoke());
        }

        public void PlayPopInAnimation(RectTransform transform, Dot dot, Action callback = null)
        {
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
    }
}