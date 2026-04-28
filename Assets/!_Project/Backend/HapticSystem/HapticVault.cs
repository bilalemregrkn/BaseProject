using Backend.Systems.Save;

namespace Backend.Systems.Haptic
{
    public sealed class HapticVault
    {
        private const string EnabledKey = "haptic_enabled";

        private readonly ISaveService _saveService;

        public bool Enabled { get; private set; }

        public HapticVault(ISaveService saveService, HapticSettings settings)
        {
            _saveService = saveService;
            Enabled = _saveService.Load(EnabledKey, settings.DefaultEnabled);
        }

        public void SaveEnabled(bool value)
        {
            Enabled = value;
            _saveService.Save(EnabledKey, value);
        }
    }
}
