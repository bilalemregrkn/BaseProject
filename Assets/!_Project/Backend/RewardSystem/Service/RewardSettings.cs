using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Backend.Systems.Reward
{
    [CreateAssetMenu(menuName = "Game/Reward Settings", fileName = "RewardSettings")]
    public class RewardSettings : ScriptableObject
    {
        [SerializeField] [InlineEditor] private List<RewardData> _rewards = new();

        public IReadOnlyList<RewardData> Rewards => _rewards;

        public RewardData GetReward(string type)
        {
            foreach (var def in _rewards)
                if (def.Id == type)
                    return def;

            return null;
        }

#if UNITY_EDITOR
        [Button]
        private void Refresh()
        {
            Game.Editor.EditorUtilities.RefreshAssets(
                owner: this,
                list: _rewards,
                getId: a => a.Id,
                generatedClassName: "RewardType",
                logTag: "RewardSystem"
            );
        }
#endif
    }
}
