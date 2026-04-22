using UnityEngine;

namespace FavTool.FavoriteAssets
{
    /// <summary>
    /// Maintains editor GUI layout state
    /// </summary>
    public struct EditorLayoutState
    {
        public GUISkin Skin;
        public Color Color;
        public Color ContentColor;
        public Color BackgroundColor;
        public bool Enabled;
        public int IndentLevel;
        public float FieldWidth;
        public float LabelWidth;
        public bool HierarchyMode;
        public bool WideMode;
    }
}