using UnityEngine;

namespace Backend.Systems.Audio
{
    public interface IAudioService
    {
        float Volume { get; set; }
        bool Muted { get; set; }

        void Play(string id, float volume = 1f, float pitch = 1f);
        void PlayAt(string id, Vector3 position, float volume = 1f);
        void Play(AudioClip clip, float volume = 1f, float pitch = 1f);
        void PlayAt(AudioClip clip, Vector3 position, float volume = 1f);
    }
}
