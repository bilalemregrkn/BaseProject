using UnityEngine;

namespace Plugins.AudioService
{
    public abstract class AudioData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private AudioClip _clip;
        [SerializeField] [Range(0f, 1f)] private float _volume = 1f;
        [SerializeField] [Range(-3f, 3f)] private float _pitch = 1f;

        public string    Id     => _id;
        public AudioClip Clip   => _clip;
        public float     Volume => _volume;
        public float     Pitch  => _pitch;

        public AudioConfig ToConfig() => new AudioConfig { Clip = _clip, Volume = _volume, Pitch = _pitch };
    }
}
