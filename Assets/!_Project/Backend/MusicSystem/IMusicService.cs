using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Backend.Systems.Music
{
    public interface IMusicService
    {
        float Volume { get; set; }
        bool Muted { get; set; }
        bool IsPlaying { get; }

        void Play(AudioClip clip, bool loop = true);
        void Stop();
        void Pause();
        void Resume();
        UniTask CrossfadeAsync(AudioClip clip, float duration = -1f, bool loop = true);
    }
}
