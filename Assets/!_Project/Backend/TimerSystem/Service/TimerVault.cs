using System;
using System.Collections.Generic;
using Backend.Systems.Save;

namespace Backend.Systems.Timer
{
    public sealed class TimerVault
    {
        private const string KeyPrefix = "timer_";
        private const string KeyList = "timer__ids";

        private readonly ISaveService _saveService;
        private readonly Dictionary<string, DateTime> _expiries = new();

        public TimerVault(ISaveService saveService)
        {
            _saveService = saveService;

            var ids = _saveService.Load(KeyList, string.Empty);
            if (string.IsNullOrEmpty(ids)) return;

            foreach (var id in ids.Split(','))
            {
                var raw = _saveService.Load(KeyPrefix + id, string.Empty);
                if (long.TryParse(raw, out var ticks) && ticks > 0)
                    _expiries[id] = new DateTime(ticks, DateTimeKind.Utc);
            }
        }

        public IEnumerable<KeyValuePair<string, DateTime>> All => _expiries;

        public bool TryGet(string id, out DateTime expiry) => _expiries.TryGetValue(id, out expiry);

        public void Set(string id, DateTime expiry)
        {
            _expiries[id] = expiry;
            _saveService.Save(KeyPrefix + id, expiry.Ticks.ToString());
            SaveIdList();
        }

        public void Remove(string id)
        {
            if (!_expiries.Remove(id)) return;
            _saveService.Delete(KeyPrefix + id);
            SaveIdList();
        }

        private void SaveIdList()
        {
            _saveService.Save(KeyList, string.Join(",", _expiries.Keys));
        }
    }
}
