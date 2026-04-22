using UnityEngine;

namespace Plugins.AudioService
{
    public sealed class AudioService : MonoBehaviour, IAudioService
    {
        private AudioVault _vault;

        public float Volume
        {
            get => _vault.Volume;
            set => _vault.SaveVolume(Mathf.Clamp01(value));
        }

        public bool Muted
        {
            get => _vault.Muted;
            set => _vault.SaveMuted(value);
        }

        public void Init(AudioVault vault) => _vault = vault;

        public void Play(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (_vault.Muted || clip == null) return;

            var source = GetPooledSource();
            source.pitch = pitch;
            source.PlayOneShot(clip, _vault.Volume * volume);
        }

        public void PlayAt(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (_vault.Muted || clip == null) return;
            AudioSource.PlayClipAtPoint(clip, position, _vault.Volume * volume);
        }

        private AudioSource GetPooledSource()
        {
            return gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        }
    }
}
