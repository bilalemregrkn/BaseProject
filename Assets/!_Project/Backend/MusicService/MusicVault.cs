using Plugins.SaveService;

namespace Plugins.MusicService
{
    public sealed class MusicVault
    {
        private const string VolumeKey = "music_volume";
        private const string MutedKey  = "music_muted";

        private readonly ISaveService _saveService;

        public float Volume { get; private set; }
        public bool Muted   { get; private set; }

        public MusicVault(ISaveService saveService, MusicSettings settings)
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
