using DG.Tweening;
using Entities.Dot;
using Interfaces;
using Interfaces.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class DoTweenManager : IDoTweenManager
    {
        public enum FadeType
        {
            FadeIn,
            FadeOut
        }
        
        private Sequence _doTweenSequence;
        
        public void PlayFadeAnimation(GameObject parent, Dot dot, FadeType fadeType)
        {
            _doTweenSequence = DOTween.Sequence();
            switch (fadeType)
            {
                case FadeType.FadeIn:
                {
                    var children = parent.GetComponentsInChildren<Graphic>();
                    foreach (var child in children)
                    {
                        var tweenAnimation = child.DOFade(1, 1).SetEase(Ease.InCubic);
                        _doTweenSequence.Join(tweenAnimation);
                    }
                    _doTweenSequence.Play();
                    break;
                }
                case FadeType.FadeOut:
                {
                    var children = parent.GetComponentsInChildren<Graphic>();
                    foreach (var child in children)
                    {
                        var tweenAnimation = child.DOFade(0, 1).SetEase(Ease.OutCubic);
                        _doTweenSequence.Join(tweenAnimation);
                    }
                    _doTweenSequence.Play();
                    break;
                }
            }
        }

        public void PlayPopOutAnimation(RectTransform transform, Dot dot)
        {
            dot.SetPendingState(true);
            _doTweenSequence = DOTween.Sequence();
            _doTweenSequence.Append(transform.DOScale(1.2f, 0.3f));
            _doTweenSequence.Append(transform.DOScale(0.9f, 0.3f));
            _doTweenSequence.Join(transform.DOScale(0, 0.5f));
            _doTweenSequence.SetEase(Ease.OutBack);
            _doTweenSequence.Play().OnComplete(() =>
            {
                dot.DisableGraphics();
                dot.SetPendingState(false);
                dot.SetActivatedState(false);
            });
        }

        public void PlayPopInAnimation(RectTransform transform, Dot dot)
        {
            dot.MoveUnderOtherDots();
            dot.EnableGraphics();
            dot.SetActivatedState(true);
            dot.SetPendingState(true);
            _doTweenSequence = DOTween.Sequence();
            _doTweenSequence.Append(transform.DOScale(0.9f, 0.5f));
            _doTweenSequence.Append(transform.DOScale(1.2f, 0.3f));
            _doTweenSequence.Append(transform.DOScale(1, 0.3f));
            _doTweenSequence.SetEase(Ease.OutBack);
            _doTweenSequence.Play().OnComplete(() => dot.SetPendingState(false));
        }

        public void PlayRippleAnimation(Dot dot, DotRipple ripple)
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