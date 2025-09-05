using System.Collections;
using Interfaces.Managers;
using Managers;
using UnityEngine;
using Zenject;

namespace Core.Timers.ZenTimer
{
    public class TimerController : MonoBehaviour
    {
        [Inject] private IGameManager gameManager;
        [Inject] private UIManager uiManager;
        
        [SerializeField] private TimerView timerView;
        private TimerModel _timerModel;

        public void StartTimer() => StartCoroutine(TimerCoroutine());
        public void StopTimer() => StopCoroutine(TimerCoroutine());
        
        private void Start()
        {
            _timerModel = new TimerModel();
            gameManager.OnRightDotClicked += IncreaseTime;
            StartTimer();
        }

        private void IncreaseTime()
        {
            _timerModel.IncreaseCurrentTime();
            UpdateView();
        }
        
        private IEnumerator TimerCoroutine()
        {
            UpdateView();
            while (true)
            {
                yield return new WaitForSeconds(1);
                
                _timerModel.DecreaseCurrentTime();
                UpdateView();

                if (_timerModel.GetCurrentTime() <= 0)
                {
                    yield return new WaitForSeconds(1);
                    break;
                }
            }

            if (_timerModel.GetCurrentTime() > 0)
            {
                StartCoroutine(TimerCoroutine());
                yield break;
            }
            gameManager?.OnLevelFailed?.Invoke();
        }

        private void UpdateView()
        {
            timerView.UpdateTimerText(_timerModel.GetCurrentTime().ToString());
        }

        private void OnDestroy()
        {
            gameManager.OnRightDotClicked -= IncreaseTime;
            StopTimer();
        }
    }
}