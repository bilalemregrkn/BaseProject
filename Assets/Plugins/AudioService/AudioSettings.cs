using UnityEngine;
using UnityEngine.Audio;

namespace Plugins.AudioService
{
    [CreateAssetMenu(menuName = "Game/Audio Settings", fileName = "AudioSettings")]
    public class AudioSettings : ScriptableObject
    {
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] [Range(0f, 1f)] private float _defaultVolume = 1f;

        public AudioMixer Mixer => _mixer;
        public float DefaultVolume => _defaultVolume;
    }
}
