using System.Collections.Generic;
using Plugins.SaveService;

namespace Plugins.CurrencyService
{
    public sealed class CurrencyVault
    {
        private const string KeyPrefix = "currency_";

        private readonly ISaveService _saveService;
        private readonly Dictionary<string, int> _amounts = new();

        public CurrencyVault(ISaveService saveService, CurrencySettings settings)
        {
            _saveService = saveService;

            foreach (var def in settings.Currencies)
                _amounts[def.Type] = _saveService.Load(KeyPrefix + def.Type, def.StartingAmount);
        }

        public int Get(string type)
        {
            if (_amounts.TryGetValue(type, out var v)) return v;
            var loaded = _saveService.Load(KeyPrefix + type, 0);
            _amounts[type] = loaded;
            return loaded;
        }

        public void Set(string type, int amount)
        {
            _amounts[type] = amount;
            _saveService.Save(KeyPrefix + type, amount);
        }
    }
}
