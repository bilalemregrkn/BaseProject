using Backend.Systems.Component;
using UnityEngine;
using UnityEngine.Audio;

namespace Backend.Systems.Music
{
    [CreateAssetMenu(menuName = "Systems/Music/Settings", fileName = "MusicSettings")]
    public class MusicSettings : BaseServiceSetting
    {
        [SerializeField] private AudioMixerGroup _mixerGroup;
        [SerializeField] [Range(0f, 1f)] private float _defaultVolume = 1f;
        [SerializeField] private float _fadeDuration = 1f;

        public AudioMixerGroup MixerGroup => _mixerGroup;
        public float DefaultVolume => _defaultVolume;
        public float FadeDuration => _fadeDuration;
    }
}
