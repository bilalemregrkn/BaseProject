using System.Collections.Generic;
using Plugins.SaveService;

namespace Plugins.CurrencyService
{
    public sealed class CurrencyVault
    {
        private const string KeyPrefix = "currency_";

        private readonly ISaveService _saveService;
        private readonly Dictionary<CurrencyType, int> _amounts = new();

        public CurrencyVault(ISaveService saveService, CurrencySettings settings)
        {
            _saveService = saveService;

            foreach (var def in settings.Definitions)
                _amounts[def.Type] = _saveService.Load(KeyPrefix + def.Type, def.StartingAmount);
        }

        public int Get(CurrencyType type) => _amounts.TryGetValue(type, out var v) ? v : 0;

        public void Set(CurrencyType type, int amount)
        {
            _amounts[type] = amount;
            _saveService.Save(KeyPrefix + type, amount);
        }
    }
}
