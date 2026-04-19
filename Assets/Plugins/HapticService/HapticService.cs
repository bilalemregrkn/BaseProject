using Plugins.SaveManagement;
using UnityEngine;

namespace Plugins.HapticService
{
    public sealed class HapticService : IHapticService
    {
        private const string EnabledKey = "haptic_enabled";

        private ISaveManager _saveManager;
        private bool _enabled;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                _saveManager.Save(EnabledKey, _enabled);
            }
        }

        public HapticService(ISaveManager saveManager)
        {
            _saveManager = saveManager;
            _enabled = _saveManager.Load(EnabledKey, true);
        }

        public void Light() => Vibrate();
        public void Medium() => Vibrate();
        public void Heavy() => Vibrate();
        public void Selection() => Vibrate();

        private void Vibrate()
        {
            if (!_enabled) return;
#if UNITY_IOS || UNITY_ANDROID
            Handheld.Vibrate();
#endif
        }
    }
}
