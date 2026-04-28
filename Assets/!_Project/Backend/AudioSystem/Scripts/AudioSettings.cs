using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace Plugins.AudioService
{
    [CreateAssetMenu(menuName = "Game/Audio Settings", fileName = "AudioSettings")]
    public class AudioSettings : ScriptableObject
    {
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] [Range(0f, 1f)] private float _defaultVolume = 1f;
        [SerializeField] [InlineEditor] private List<AudioData> _clips = new();

        public AudioMixer Mixer => _mixer;
        public float DefaultVolume => _defaultVolume;
        public IReadOnlyList<AudioData> Clips => _clips;

        public AudioData GetData(string id)
        {
            foreach (var clip in _clips)
                if (clip.Id == id) return clip;
            return null;
        }

#if UNITY_EDITOR
        [Button]
        private void Refresh()
        {
            Game.Editor.EditorUtilities.RefreshAssets(
                owner: this,
                list: _clips,
                getId: a => a.Id,
                generatedClassName: "AudioType",
                logTag: "AudioSettings"
            );
        }
#endif
    }
}
