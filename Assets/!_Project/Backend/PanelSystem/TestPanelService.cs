using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.PanelService
{
    public class TestPanelService : MonoBehaviour
    {
        [Inject] private IPanelService _service;

        [ValueDropdown(nameof(GetPanelIds))]
        public string id;

#if UNITY_EDITOR
        private static IEnumerable<string> GetPanelIds()
        {
            var settings = UnityEditor.AssetDatabase.FindAssets("t:PanelServiceSettings");
            foreach (var guid in settings)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<PanelServiceSettings>(path);
                if (asset == null) continue;
                foreach (var panel in asset.Panels)
                    if (!string.IsNullOrEmpty(panel.Id))
                        yield return panel.Id;
                yield break;
            }
        }
#endif

        [Button]
        public void Show()
        {
            _service.ShowAsync(id).Forget();
        }

        [Button]
        public void Hide()
        {
            _service.HideAsync(id).Forget();
        }
    }
}