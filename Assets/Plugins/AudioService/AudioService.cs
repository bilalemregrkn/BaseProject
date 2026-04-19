using Plugins.SaveService;
using UnityEngine;

namespace Plugins.AudioService
{
    public sealed class AudioService : MonoBehaviour, IAudioService
    {
        private const string VolumeKey = "sfx_volume";
        private const string MutedKey = "sfx_muted";

        private ISaveService _saveManager;
        private float _volume = 1f;
        private bool _muted;

        public float Volume
        {
            get => _volume;
            set
            {
                _volume = Mathf.Clamp01(value);
                _saveManager.Save(VolumeKey, _volume);
            }
        }

        public bool Muted
        {
            get => _muted;
            set
            {
                _muted = value;
                _saveManager.Save(MutedKey, _muted);
            }
        }

        public void Init(ISaveService saveManager)
        {
            _saveManager = saveManager;
            _volume = _saveManager.Load(VolumeKey, 1f);
            _muted = _saveManager.Load(MutedKey, false);
        }

        public void Play(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (_muted || clip == null) return;

            var source = GetPooledSource();
            source.pitch = pitch;
            source.PlayOneShot(clip, _volume * volume);
        }

        public void PlayAt(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (_muted || clip == null) return;
            AudioSource.PlayClipAtPoint(clip, position, _volume * volume);
        }

        private AudioSource GetPooledSource()
        {
            // Single shared source is enough for fire-and-forget SFX via PlayOneShot
            return gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        }
    }
}
