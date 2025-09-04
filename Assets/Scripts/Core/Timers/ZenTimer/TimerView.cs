using TMPro;
using UnityEngine;

namespace Core.Timers.ZenTimer
{
    public class TimerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        
        public void UpdateTimerText(string text) => timerText.text = text;
    }
}