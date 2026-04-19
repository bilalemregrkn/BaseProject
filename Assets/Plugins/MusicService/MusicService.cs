using Cysharp.Threading.Tasks;
using Plugins.SaveManagement;
using PrimeTween;
using UnityEngine;

namespace Plugins.MusicService
{
    public sealed class MusicService : MonoBehaviour, IMusicService
    {
        private const string VolumeKey = "music_volume";
        private const string MutedKey = "music_muted";

        private ISaveManager _saveManager;
        private AudioSource _sourceA;
        private AudioSource _sourceB;
        private bool _usingA = true;
        private float _volume = 1f;
        private bool _muted;

        public float Volume
        {
            get => _volume;
            set
            {
                _volume = Mathf.Clamp01(value);
                ActiveSource.volume = _muted ? 0f : _volume;
                _saveManager.Save(VolumeKey, _volume);
            }
        }

        public bool Muted
        {
            get => _muted;
            set
            {
                _muted = value;
                ActiveSource.volume = _muted ? 0f : _volume;
                _saveManager.Save(MutedKey, _muted);
            }
        }

        public bool IsPlaying => ActiveSource.isPlaying;

        private AudioSource ActiveSource => _usingA ? _sourceA : _sourceB;
        private AudioSource InactiveSource => _usingA ? _sourceB : _sourceA;

        public void Init(ISaveManager saveManager)
        {
            _saveManager = saveManager;
            _volume = _saveManager.Load(VolumeKey, 1f);
            _muted = _saveManager.Load(MutedKey, false);

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
            source.volume = _muted ? 0f : _volume;
            source.Play();
        }

        public void Stop()
        {
            ActiveSource.Stop();
            ActiveSource.clip = null;
        }

        public void Pause() => ActiveSource.Pause();

        public void Resume() => ActiveSource.UnPause();

        public async UniTask CrossfadeAsync(AudioClip clip, float duration = 1f, bool loop = true)
        {
            if (clip == null) return;

            var outSource = ActiveSource;
            _usingA = !_usingA;
            var inSource = ActiveSource;

            inSource.clip = clip;
            inSource.loop = loop;
            inSource.volume = 0f;
            inSource.Play();

            var targetVolume = _muted ? 0f : _volume;

            await UniTask.WhenAll(
                Tween.AudioVolume(outSource, 0f, duration).ToUniTask(),
                Tween.AudioVolume(inSource, targetVolume, duration).ToUniTask()
            );

            outSource.Stop();
            outSource.clip = null;
        }
    }
}
