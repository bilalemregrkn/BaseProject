using Plugins.EventBus;
using UnityEngine;

namespace Plugins.AudioService
{
    public sealed class AudioService : IAudioService
    {
        private readonly IEventBus _eventBus;
        private readonly AudioVault _vault;
        private readonly AudioPlayer _player;
        private readonly AudioSettings _settings;

        public AudioService(IEventBus eventBus, AudioVault vault, AudioPlayer player, AudioSettings settings)
        {
            _eventBus = eventBus;
            _vault = vault;
            _player = player;
            _settings = settings;
        }

        public float Volume
        {
            get => _vault.Volume;
            set
            {
                var clamped = Mathf.Clamp01(value);
                var old = _vault.Volume;
                if (Mathf.Approximately(old, clamped)) return;
                _vault.SaveVolume(clamped);
                _eventBus.Publish(new VolumeChanged { OldVolume = old, NewVolume = clamped });
            }
        }

        public bool Muted
        {
            get => _vault.Muted;
            set
            {
                if (_vault.Muted == value) return;
                _vault.SaveMuted(value);
                _eventBus.Publish(new MuteChanged { Muted = value });
            }
        }

        public void Play(string id, float volume = 1f, float pitch = 1f)
        {
            var data = _settings.GetData(id);
            if (data == null) return;
            var config = data.ToConfig();
            Play(config.Clip, config.Volume * volume, config.Pitch * pitch);
        }

        public void PlayAt(string id, Vector3 position, float volume = 1f)
        {
            var data = _settings.GetData(id);
            if (data == null) return;
            var config = data.ToConfig();
            PlayAt(config.Clip, position, config.Volume * volume);
        }

        public void Play(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (_vault.Muted || clip == null) return;
            _player.Play(clip, _vault.Volume * volume, pitch);
        }

        public void PlayAt(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (_vault.Muted || clip == null) return;
            _player.PlayAt(clip, position, _vault.Volume * volume);
        }
    }
}
