using System.Collections.Generic;
using UnityEngine;

namespace Plugins.AudioService
{
    public sealed class AudioPlayer : MonoBehaviour
    {
        private readonly List<AudioSource> _pool = new();

        public void Play(AudioClip clip, float volume, float pitch)
        {
            var source = GetFreeSource();
            source.pitch = pitch;
            source.PlayOneShot(clip, volume);
        }

        public void PlayAt(AudioClip clip, Vector3 position, float volume)
        {
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }

        private AudioSource GetFreeSource()
        {
            foreach (var s in _pool)
                if (!s.isPlaying) return s;

            var source = gameObject.AddComponent<AudioSource>();
            _pool.Add(source);
            return source;
        }
    }
}
