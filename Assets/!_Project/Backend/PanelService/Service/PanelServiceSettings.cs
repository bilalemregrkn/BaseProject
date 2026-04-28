using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.PanelService
{
    [CreateAssetMenu(menuName = "Game/Panel Settings", fileName = "PanelSettings")]
    public class PanelServiceSettings : ScriptableObject
    {
        [SerializeField] [InlineEditor] private List<PanelData> _panels = new();

        public IReadOnlyList<PanelData> Panels => _panels;

        public PanelBase GetPrefab(string id)
        {
            foreach (var data in _panels)
                if (data.Id == id) return data.Prefab;
            return null;
        }

#if UNITY_EDITOR
        [Button]
        private void Refresh()
        {
            Game.Editor.EditorUtilities.RefreshAssets(
                owner: this,
                list: _panels,
                getId: a => a.Id,
                generatedClassName: "PanelType",
                logTag: "PanelService"
            );
        }
#endif
    }
}
