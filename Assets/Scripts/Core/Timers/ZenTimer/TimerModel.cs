namespace Core.Timers.ZenTimer
{
    public class TimerModel
    {
        private const int TimeReward = 1;
        private const int StartTime = 5;
        private int currentTime = StartTime;
        
        public int GetCurrentTime() => currentTime;
        
        public void IncreaseCurrentTime() => currentTime += TimeReward;
        public void DecreaseCurrentTime() => currentTime--;
    }
}
