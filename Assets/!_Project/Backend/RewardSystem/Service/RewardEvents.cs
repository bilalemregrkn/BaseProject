using Backend.Systems.EventBus;

namespace Backend.Systems.Reward
{
    public struct RewardGranted : IEvent
    {
        public string Type;
        public int Amount;
    }
}
