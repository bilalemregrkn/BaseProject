using System;
using UnityEditor;

namespace FavTool.FavoriteAssets
{
    /// <summary>
    /// Represents the state of an inline asset editor
    /// </summary>
    [Serializable]
    public class AssetEditorState
    {
        // Properties
        public UnityEngine.Object Target;
        public Editor EditorInstance;
        public Editor PreviewEditorInstance;
        public bool IsExpanded;
        public bool IsFoldedOut = true;
        public bool ShouldDrawHeader = true;
        public bool ShouldDrawGUI = true;
        public bool ShouldDrawPreview = true;
    }
}