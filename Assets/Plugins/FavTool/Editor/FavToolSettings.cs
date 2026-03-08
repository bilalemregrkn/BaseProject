using UnityEngine;
using UnityEditor;

namespace FavTool.FavoriteAssets
{
    public class FavToolSettings : ScriptableObject
    {
        private const string SettingsPath = "Assets/Plugins/FavTool/Editor/FavToolSettings.asset";

        [Header("Row Colors")]
        public Color EvenRowColor = new Color(0.24f, 0.28f, 0.32f, 1.0f);
        public Color OddRowColor = new Color(0.20f, 0.22f, 0.24f, 1.0f);

        private static FavToolSettings _instance;

        public static FavToolSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = AssetDatabase.LoadAssetAtPath<FavToolSettings>(SettingsPath);

                    if (_instance == null)
                    {
                        _instance = CreateInstance<FavToolSettings>();
                        AssetDatabase.CreateAsset(_instance, SettingsPath);
                        AssetDatabase.SaveAssets();
                    }
                }

                return _instance;
            }
        }

        [MenuItem("Tools/Favorite Assets Settings")]
        public static void SelectSettings()
        {
            Selection.activeObject = Instance;
        }
    }
}
