using System.Collections.Generic;
using Backend.Systems.Save;

namespace Backend.Systems.Reward
{
    public sealed class RewardVault
    {
        private const string KeyPrefix = "reward_";

        private readonly ISaveService _saveService;
        private readonly Dictionary<string, int> _pending = new();

        public RewardVault(ISaveService saveService, RewardSettings settings)
        {
            _saveService = saveService;

            foreach (var def in settings.Rewards)
                _pending[def.Id] = _saveService.Load(KeyPrefix + def.Id, 0);
        }

        public int Get(string type)
        {
            if (_pending.TryGetValue(type, out var v)) return v;
            var loaded = _saveService.Load(KeyPrefix + type, 0);
            _pending[type] = loaded;
            return loaded;
        }

        public void Set(string type, int amount)
        {
            _pending[type] = amount;
            _saveService.Save(KeyPrefix + type, amount);
        }
    }
}
