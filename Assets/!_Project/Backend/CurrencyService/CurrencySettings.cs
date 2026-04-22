using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plugins.CurrencyService
{
    [CreateAssetMenu(menuName = "Game/Currency Settings", fileName = "CurrencySettings")]
    public class CurrencySettings : ScriptableObject
    {
        [SerializeField] private List<CurrencyDefinition> _definitions = new();

        public IReadOnlyList<CurrencyDefinition> Definitions => _definitions;

        public CurrencyDefinition GetDefinition(CurrencyType type)
        {
            foreach (var def in _definitions)
                if (def.Type == type) return def;

            return new CurrencyDefinition { Type = type, StartingAmount = 0, MaxAmount = 0 };
        }
    }

    [Serializable]
    public struct CurrencyDefinition
    {
        public CurrencyType Type;
        public string DisplayName;
        public Sprite Icon;
        public int StartingAmount;
        [Tooltip("0 = uncapped")]
        public int MaxAmount;
    }
}
