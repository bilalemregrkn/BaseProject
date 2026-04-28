using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace Backend.Systems.Music
{
    public sealed class MusicService : MonoBehaviour, IMusicService
    {
        private MusicVault _vault;
        private MusicSettings _settings;
        private AudioSource _sourceA;
        private AudioSource _sourceB;
        private bool _usingA = true;

        public float Volume
        {
            get => _vault.Volume;
            set
            {
                _vault.SaveVolume(Mathf.Clamp01(value));
                ActiveSource.volume = _vault.Muted ? 0f : _vault.Volume;
            }
        }

        public bool Muted
        {
            get => _vault.Muted;
            set
            {
                _vault.SaveMuted(value);
                ActiveSource.volume = _vault.Muted ? 0f : _vault.Volume;
            }
        }

        public bool IsPlaying => ActiveSource.isPlaying;

        private AudioSource ActiveSource   => _usingA ? _sourceA : _sourceB;
        private AudioSource InactiveSource => _usingA ? _sourceB : _sourceA;

        public void Init(MusicVault vault, MusicSettings settings)
        {
            _vault = vault;
            _settings = settings;

            _sourceA = gameObject.AddComponent<AudioSource>();
            _sourceB = gameObject.AddComponent<AudioSource>();
            _sourceA.playOnAwake = false;
            _sourceB.playOnAwake = false;
        }

        public void Play(AudioClip clip, bool loop = true)
        {
            if (clip == null) return;

            var source = ActiveSource;
            source.clip = clip;
            source.loop = loop;
            source.volume = _vault.Muted ? 0f : _vault.Volume;
            source.Play();
        }

        public void Stop()
        {
            ActiveSource.Stop();
            ActiveSource.clip = null;
        }

        public void Pause()  => ActiveSource.Pause();
        public void Resume() => ActiveSource.UnPause();

        public async UniTask CrossfadeAsync(AudioClip clip, float duration = -1f, bool loop = true)
        {
            if (clip == null) return;

            var fadeDuration = duration < 0f ? _settings.FadeDuration : duration;
            var outSource = ActiveSource;
            _usingA = !_usingA;
            var inSource = ActiveSource;

            inSource.clip = clip;
            inSource.loop = loop;
            inSource.volume = 0f;
            inSource.Play();

            var targetVolume = _vault.Muted ? 0f : _vault.Volume;

            await UniTask.WhenAll(
                Tween.AudioVolume(outSource, 0f, fadeDuration).ToUniTask(),
                Tween.AudioVolume(inSource, targetVolume, fadeDuration).ToUniTask()
            );

            outSource.Stop();
            outSource.clip = null;
        }
    }
}
