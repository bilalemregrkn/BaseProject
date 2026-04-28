using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.CurrencyService
{
    [CreateAssetMenu(menuName = "Game/Currency Settings", fileName = "CurrencySettings")]
    public class CurrencySettings : ScriptableObject
    {
        [SerializeField] [InlineEditor] private List<CurrencyData> currencies = new();

        public IReadOnlyList<CurrencyData> Currencies => currencies;

        public CurrencyData GetCurrency(string type)
        {
            foreach (var def in currencies)
                if (def.Id == type)
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
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<CurrencyData>(path);
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
                var raw = System.Text.RegularExpressions.Regex.Replace(c.Id, @"[^a-zA-Z0-9_]", "_");
                var identifier = char.ToUpper(raw[0]) + raw.Substring(1);
                sb.AppendLine($"        public const string {identifier} = \"{c.Id}\";");
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