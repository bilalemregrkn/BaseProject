using UnityEngine;

namespace Plugins.HapticService
{
    public sealed class HapticService : IHapticService
    {
        private readonly HapticVault _vault;

        public bool Enabled
        {
            get => _vault.Enabled;
            set => _vault.SaveEnabled(value);
        }

        public HapticService(HapticVault vault) => _vault = vault;

        public void Light()     => Vibrate();
        public void Medium()    => Vibrate();
        public void Heavy()     => Vibrate();
        public void Selection() => Vibrate();

        private void Vibrate()
        {
            if (!_vault.Enabled) return;
#if UNITY_IOS || UNITY_ANDROID
            Handheld.Vibrate();
#endif
        }
    }
}
