using UnityEngine;

namespace Plugins.CurrencyService
{
    public abstract class BaseCurrency : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _startingAmount;
        [Tooltip("0 = uncapped")]
        [SerializeField] private int _maxAmount;

        public string Id           => _id;
        public string DisplayName  => _displayName;
        public Sprite Icon         => _icon;
        public int StartingAmount  => _startingAmount;
        public int MaxAmount       => _maxAmount;
    }
}
