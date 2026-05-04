using System;

namespace Backend.Systems.Timer
{
    public class Deadline
    {
        private DateTime _expiry;
        private bool _initialized;

        public bool IsRunning => _initialized && DateTime.UtcNow < _expiry;
        public bool IsExpired => _initialized && DateTime.UtcNow >= _expiry;

        public void Init(TimeSpan duration)
        {
            _expiry = DateTime.UtcNow + duration;
            _initialized = true;
        }

        public void Load(DateTime expiry)
        {
            _expiry = expiry;
            _initialized = true;
        }

        public TimeSpan GetRemainTime()
        {
            if (!_initialized) return TimeSpan.Zero;
            var remaining = _expiry - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }

        public void Cancel()
        {
            _initialized = false;
        }
    }
}