using Audio;
using Entities.Dot;
using Interfaces;
using Managers;
using UnityEngine;
using Zenject;

namespace Buttons.DefaultMode
{
    public class DotButton : UIButton
    {
        [Inject] private IGameManager _gameManager;
        [Inject] private DoTweenManager _doTweenManager;
        [Inject] private AudioManager _audioManager;
        
        [SerializeField] private Dot dot;
        [SerializeField] private SoundLibrary soundLibrary;

        protected override void HandleButton()
        {
            _gameManager.TrySelect(dot);
        }

        public void PlayCorrectSequence()
        {
            _audioManager.PlaySound(dot.GetAudioSource(), soundLibrary.popSound);
            _doTweenManager.PlayPopAnimation(dot.GetTransform());
        }
        
        public void PlayWrongSequence()
        {
            _doTweenManager.PlayRippleAnimation(dot, dot.GetRipple());
        }
    }
}
