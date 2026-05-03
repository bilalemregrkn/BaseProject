using System;
using System.Collections.Generic;
using Backend.Systems.EventBus;
using Backend.Systems.Update;

namespace Backend.Systems.Timer
{
    public sealed class TimerService : ITimerService, IUpdatable
    {
        private readonly IEventBus _eventBus;
        private readonly TimerVault _vault;
        private readonly List<string> _expiredBuffer = new();

        public TimerService(IEventBus eventBus, TimerVault vault, IUpdateService updateService)
        {
            _eventBus = eventBus;
            _vault = vault;
            updateService.Add(this);
        }

        public void Tick(float dt)
        {
            var now = DateTime.UtcNow;
            foreach (var pair in _vault.All)
            {
                if (now >= pair.Value)
                    _expiredBuffer.Add(pair.Key);
            }

            foreach (var id in _expiredBuffer)
            {
                _vault.Remove(id);
                _eventBus.Publish(new TimerExpired { Id = id });
            }

            _expiredBuffer.Clear();
        }

        public void Start(string id, TimeSpan duration)
        {
            var expiry = DateTime.UtcNow + duration;
            _vault.Set(id, expiry);
            _eventBus.Publish(new TimerStarted { Id = id, Expiry = expiry });
        }

        public void Cancel(string id)
        {
            if (!_vault.TryGet(id, out _)) return;
            _vault.Remove(id);
            _eventBus.Publish(new TimerCancelled { Id = id });
        }

        public bool IsRunning(string id) =>
            _vault.TryGet(id, out var expiry) && DateTime.UtcNow < expiry;

        public bool IsExpired(string id) =>
            _vault.TryGet(id, out var expiry) && DateTime.UtcNow >= expiry;

        public TimeSpan GetRemaining(string id)
        {
            if (!_vault.TryGet(id, out var expiry)) return TimeSpan.Zero;
            var remaining = expiry - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }

        public DateTime GetExpiry(string id)
        {
            _vault.TryGet(id, out var expiry);
            return expiry;
        }
    }
}
