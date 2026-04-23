using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.CurrencyService
{
    [CreateAssetMenu(menuName = "Game/Currency Settings", fileName = "CurrencySettings")]
    public class CurrencySettings : ScriptableObject
    {
        [SerializeField] [InlineEditor] private List<BaseCurrency> currencies = new();

        public IReadOnlyList<BaseCurrency> Currencies => currencies;

        public BaseCurrency GetCurrency(string type)
        {
            foreach (var def in currencies)
                if (def.Type == type)
                    return def;

            return null;
        }

#if UNITY_EDITOR
        [Button]
        private void Refresh()
        {
            currencies.Clear();

            var guids = UnityEditor.AssetDatabase.FindAssets("t:BaseCurrency");
            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<BaseCurrency>(path);
                if (asset != null)
                    currencies.Add(asset);
            }

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);

            RegenerateCurrencyTypeFile();

            Debug.Log($"[CurrencySettings] Refreshed {currencies.Count} BaseCurrency asset(s).");
        }

        private void RegenerateCurrencyTypeFile()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("namespace Plugins.CurrencyService");
            sb.AppendLine("{");
            sb.AppendLine("    public static class CurrencyType");
            sb.AppendLine("    {");
            foreach (var c in currencies)
            {
                var identifier = System.Text.RegularExpressions.Regex.Replace(c.Type, @"[^a-zA-Z0-9_]", "_");
                sb.AppendLine($"        public const string {identifier} = \"{c.Type}\";");
            }
            sb.AppendLine("    }");
            sb.Append("}");

            const string assetPath = "Assets/!_Project/Backend/CurrencyService/CurrencyType.cs";
            System.IO.File.WriteAllText(
                System.IO.Path.Combine(UnityEngine.Application.dataPath, "../" + assetPath),
                sb.ToString()
            );
            UnityEditor.AssetDatabase.ImportAsset(assetPath);
        }
#endif
    }
}