using System;
using System.Collections.Generic;
using Plugins.EventBus;
using Plugins.SaveService;

namespace Plugins.CurrencyService
{
    public sealed class CurrencyService : ICurrencyService
    {
        private const string SaveKeyPrefix = "currency_";

        private readonly IEventBus _eventBus;
        private readonly ISaveService _saveService;
        private readonly Dictionary<CurrencyType, int> _amounts = new();

        public CurrencyService(IEventBus eventBus, ISaveService saveService)
        {
            _eventBus = eventBus;
            _saveService = saveService;

            foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
                _amounts[type] = _saveService.Load(SaveKeyPrefix + type, 0);
        }

        public int Get(CurrencyType type) => _amounts[type];

        public void Add(CurrencyType type, int amount)
        {
            if (amount <= 0) return;
            SetInternal(type, _amounts[type] + amount);
        }

        public bool TrySpend(CurrencyType type, int amount)
        {
            if (!Has(type, amount))
            {
                _eventBus.Publish(new CurrencyInsufficient
                {
                    Type = type,
                    Required = amount,
                    Available = _amounts[type]
                });
                return false;
            }

            SetInternal(type, _amounts[type] - amount);
            return true;
        }

        public bool Has(CurrencyType type, int amount) => _amounts[type] >= amount;

        public void Set(CurrencyType type, int amount) => SetInternal(type, amount);

        private void SetInternal(CurrencyType type, int newAmount)
        {
            newAmount = newAmount < 0 ? 0 : newAmount;
            var old = _amounts[type];
            if (old == newAmount) return;

            _amounts[type] = newAmount;
            _saveService.Save(SaveKeyPrefix + type, newAmount);
            _eventBus.Publish(new CurrencyChanged { Type = type, OldAmount = old, NewAmount = newAmount });
        }
    }
}
