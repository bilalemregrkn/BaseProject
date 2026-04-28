using Backend.Systems.EventBus;

namespace Backend.Systems.Reward
{
    public sealed class RewardService : IRewardService
    {
        private readonly IEventBus _eventBus;
        private readonly RewardVault _vault;

        public RewardService(IEventBus eventBus, RewardVault vault)
        {
            _eventBus = eventBus;
            _vault = vault;
        }

        public void Grant(string type, int amount)
        {
            if (amount <= 0) return;
            _vault.Set(type, _vault.Get(type) + amount);
            _eventBus.Publish(new RewardGranted { Type = type, Amount = amount });
        }

        public bool TryClaim(string type, out int amount)
        {
            amount = _vault.Get(type);
            if (amount <= 0) return false;
            _vault.Set(type, 0);
            return true;
        }

        public bool HasPending(string type) => _vault.Get(type) > 0;
    }
}
