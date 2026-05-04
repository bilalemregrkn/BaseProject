using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Backend.Systems.Component
{
    [CreateAssetMenu(menuName = "Systems/AllSettings", fileName = "AllSettings")]
    public class AllSettings : ScriptableObject
    {
        [SerializeField] [InlineEditor] private List<BaseServiceSetting> _settings = new();

        public IReadOnlyList<BaseServiceSetting> Settings => _settings;

        public T Get<T>() where T : BaseServiceSetting
        {
            foreach (var setting in _settings)
                if (setting is T t) return t;
            return null;
        }

#if UNITY_EDITOR
        [Button]
        private void Fetch()
        {
            _settings.Clear();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(BaseServiceSetting)}");
            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<BaseServiceSetting>(path);
                if (asset != null) _settings.Add(asset);
            }
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
            UnityEngine.Debug.Log($"[AllSettings] Fetched {_settings.Count} settings.");
        }
#endif
    }
}
