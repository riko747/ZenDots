using Audio;
using Entities.Dot;
using Interfaces.Managers;
using UnityEngine;
using Zenject;

namespace Buttons.DefaultMode
{
    public class DotButton : UIButton
    {
        [Inject] private IGameManager _gameManager;
        [Inject] private IDoTweenManager _doTweenManager;
        [Inject] private IAudioManager _audioManager;
        
        [SerializeField] private Dot dot;
        [SerializeField] private SoundLibrary soundLibrary;

        protected override void HandleButton()
        {
            _gameManager.ValidateStrategy.Validate(dot);
        }

        public void PlayCorrectSequence()
        {
            _audioManager.PlaySound(dot.GetAudioSource(), soundLibrary.popSound);
            dot.SetPendingState(true);
            _doTweenManager.PlayPopOutAnimation(dot.GetTransform(), dot, () => dot.SetActivatedState(true));
        }
        
        public void PlayWrongSequence()
        {
            _doTweenManager.PlayRippleAnimation(dot, dot.GetRipple());
        }
    }
}
