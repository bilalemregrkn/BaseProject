using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Backend.Systems.Scene
{
    [CreateAssetMenu(menuName = "Game/Scene Settings", fileName = "SceneSettings")]
    public class SceneSettings : ScriptableObject
    {
        [SerializeField] [InlineEditor] private List<SceneData> _scenes = new();

        public IReadOnlyList<SceneData> Scenes => _scenes;

        public SceneData GetData(string id)
        {
            foreach (var scene in _scenes)
                if (scene.Id == id) return scene;
            return null;
        }

#if UNITY_EDITOR
        [Button]
        private void Refresh()
        {
            Game.Editor.EditorUtilities.RefreshAssets(
                owner: this,
                list: _scenes,
                getId: s => s.Id,
                generatedClassName: "SceneType",
                logTag: "SceneSettings"
            );
        }
#endif
    }
}
