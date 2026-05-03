using System;

namespace Backend.Systems.Timer
{
    public interface ITimerService
    {
        void Start(string id, TimeSpan duration);
        void Cancel(string id);
        bool IsRunning(string id);
        bool IsExpired(string id);
        TimeSpan GetRemaining(string id);
        DateTime GetExpiry(string id);
    }
}
