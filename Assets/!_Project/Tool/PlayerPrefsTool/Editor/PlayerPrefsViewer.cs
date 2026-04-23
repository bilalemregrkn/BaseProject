using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Tool.PlayerPrefsTool
{
    public class PlayerPrefsViewer : EditorWindow
    {
        private Vector2 _scroll;
        private string _search = "";
        private List<(string key, string value)> _entries = new();

        [MenuItem("Tools/PlayerPrefs Viewer")]
        public static void Open() => GetWindow<PlayerPrefsViewer>("PlayerPrefs Viewer");

        private void OnEnable() => Refresh();

        private void OnGUI()
        {
            DrawToolbar();
            DrawEntries();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            _search = EditorGUILayout.TextField(_search, EditorStyles.toolbarSearchField, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
                Refresh();

            if (GUILayout.Button("Delete All", EditorStyles.toolbarButton, GUILayout.Width(70)))
            {
                if (EditorUtility.DisplayDialog("Delete All PlayerPrefs", "Are you sure?", "Delete", "Cancel"))
                {
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();
                    Refresh();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawEntries()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            var filter = _search.ToLower();
            bool odd = false;

            foreach (var (key, value) in _entries)
            {
                if (!string.IsNullOrEmpty(filter) &&
                    !key.ToLower().Contains(filter) &&
                    !value.ToLower().Contains(filter))
                    continue;

                var rect = EditorGUILayout.BeginHorizontal();
                var bg = odd ? new Color(0.22f, 0.22f, 0.22f) : new Color(0.18f, 0.18f, 0.18f);
                EditorGUI.DrawRect(rect, bg);

                EditorGUILayout.LabelField(key, GUILayout.Width(220));
                EditorGUILayout.LabelField(value, GUILayout.ExpandWidth(true));

                if (GUILayout.Button("X", GUILayout.Width(22)))
                {
                    PlayerPrefs.DeleteKey(key);
                    PlayerPrefs.Save();
                    Refresh();
                    EditorGUILayout.EndHorizontal();
                    break;
                }

                EditorGUILayout.EndHorizontal();
                odd = !odd;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.LabelField($"{_entries.Count} keys total", EditorStyles.miniLabel);
        }

        private void Refresh()
        {
            _entries.Clear();

#if UNITY_EDITOR_WIN
            ReadWindows();
#elif UNITY_EDITOR_OSX
            ReadMacOS();
#endif
        }

#if UNITY_EDITOR_WIN
        private void ReadWindows()
        {
            var path = $@"Software\Unity\UnityEditor\{PlayerSettings.companyName}\{PlayerSettings.productName}";
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path);
            if (key == null) return;

            foreach (var name in key.GetValueNames())
            {
                // Registry names have a hash suffix like _hXXXXXXXX — strip it
                var cleanKey = System.Text.RegularExpressions.Regex.Replace(name, @"_h\d+$", "");
                var val = key.GetValue(name);
                _entries.Add((cleanKey, val?.ToString() ?? ""));
            }
        }
#elif UNITY_EDITOR_OSX
        private void ReadMacOS()
        {
            var company = PlayerSettings.companyName;
            var product = PlayerSettings.productName;
            var bundleId = $"unity.{company}.{product}";

            var psi = new ProcessStartInfo("defaults", $"read {bundleId}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            var output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            // Parse "key = value;" lines
            foreach (var line in output.Split('\n'))
            {
                var trimmed = line.Trim().TrimEnd(';');
                var eq = trimmed.IndexOf(" = ");
                if (eq < 0) continue;

                var k = trimmed.Substring(0, eq).Trim();
                var v = trimmed.Substring(eq + 3).Trim().Trim('"');
                _entries.Add((k, v));
            }
        }
#endif
    }
}
