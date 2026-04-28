using Backend.Systems.Save;

namespace Backend.Systems.Audio
{
    public sealed class AudioVault
    {
        private const string VolumeKey = "sfx_volume";
        private const string MutedKey  = "sfx_muted";

        private readonly ISaveService _saveService;

        public float Volume { get; private set; }
        public bool Muted   { get; private set; }

        public AudioVault(ISaveService saveService, AudioSettings settings)
        {
            _saveService = saveService;
            Volume = _saveService.Load(VolumeKey, settings.DefaultVolume);
            Muted  = _saveService.Load(MutedKey,  false);
        }

        public void SaveVolume(float value)
        {
            Volume = value;
            _saveService.Save(VolumeKey, value);
        }

        public void SaveMuted(bool value)
        {
            Muted = value;
            _saveService.Save(MutedKey, value);
        }
    }
}
