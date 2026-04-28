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

        public int Get(string type) => _vault.Get(type);

        public void Add(string type, int amount)
        {
            if (amount <= 0) return;
            SetInternal(type, _vault.Get(type) + amount);
        }

        public bool TrySpend(string type, int amount)
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

        public bool Has(string type, int amount) => _vault.Get(type) >= amount;

        public void Set(string type, int amount) => SetInternal(type, amount);

        private void SetInternal(string type, int newAmount)
        {
            var def = _settings.GetCurrency(type);
            if (def != null && def.MaxAmount > 0 && newAmount > def.MaxAmount)
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
