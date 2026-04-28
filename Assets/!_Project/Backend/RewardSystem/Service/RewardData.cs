using UnityEngine;

namespace Backend.Systems.Reward
{
    public abstract class RewardData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;

        public string Id          => _id;
        public string DisplayName => _displayName;
        public Sprite Icon        => _icon;
    }
}
