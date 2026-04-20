using Plugins.EventBus;

namespace Plugins.CurrencyService
{
    public sealed class CurrencyService : ICurrencyService
    {
        private readonly IEventBus _eventBus;
        private readonly CurrencyVault _vault;
        private readonly CurrencySettings _settings;

        public CurrencyService(IEventBus eventBus, CurrencyVault vault, CurrencySettings settings)
        {
            _eventBus = eventBus;
            _vault = vault;
            _settings = settings;
        }

        public int Get(CurrencyType type) => _vault.Get(type);

        public void Add(CurrencyType type, int amount)
        {
            if (amount <= 0) return;
            SetInternal(type, _vault.Get(type) + amount);
        }

        public bool TrySpend(CurrencyType type, int amount)
        {
            if (!Has(type, amount))
            {
                _eventBus.Publish(new CurrencyInsufficient
                {
                    Type = type,
                    Required = amount,
                    Available = _vault.Get(type)
                });
                return false;
            }

            SetInternal(type, _vault.Get(type) - amount);
            return true;
        }

        public bool Has(CurrencyType type, int amount) => _vault.Get(type) >= amount;

        public void Set(CurrencyType type, int amount) => SetInternal(type, amount);

        private void SetInternal(CurrencyType type, int newAmount)
        {
            var def = _settings.GetDefinition(type);
            if (def.MaxAmount > 0 && newAmount > def.MaxAmount)
                newAmount = def.MaxAmount;
            if (newAmount < 0)
                newAmount = 0;

            var old = _vault.Get(type);
            if (old == newAmount) return;

            _vault.Set(type, newAmount);
            _eventBus.Publish(new CurrencyChanged { Type = type, OldAmount = old, NewAmount = newAmount });
        }
    }
}
