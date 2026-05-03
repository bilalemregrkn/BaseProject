using System;
using Backend.Systems.EventBus;

namespace Backend.Systems.Timer
{
    public struct TimerStarted : IEvent
    {
        public string Id;
        public DateTime Expiry;
    }

    public struct TimerExpired : IEvent
    {
        public string Id;
    }

    public struct TimerCancelled : IEvent
    {
        public string Id;
    }
}
