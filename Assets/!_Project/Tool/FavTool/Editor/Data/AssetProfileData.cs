using System;
using System.Collections.Generic;

namespace FavTool.FavoriteAssets
{
    /// <summary>
    /// Represents the data structure for serializing asset profile information
    /// </summary>
    [Serializable]
    public class AssetProfileData
    {
        public List<string> AssetPaths;
        public List<string> ExpandedAssetPaths;
        public List<bool> FoldedOutStates;
        public float AssetsScrollHeight = 300f; // Default height
    }
}