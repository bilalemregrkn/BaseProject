using UnityEngine;

namespace Plugins.AudioService
{
    public interface IAudioService
    {
        float Volume { get; set; }
        bool Muted { get; set; }

        void Play(AudioClip clip, float volume = 1f, float pitch = 1f);
        void PlayAt(AudioClip clip, Vector3 position, float volume = 1f);
    }
}
