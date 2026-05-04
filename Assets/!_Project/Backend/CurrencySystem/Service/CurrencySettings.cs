using System.Collections.Generic;
using Backend.Systems.Component;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Backend.Systems.Currency
{
    [CreateAssetMenu(menuName = "Systems/Currency/Settings", fileName = "CurrencySettings")]
    public class CurrencySettings : BaseServiceSetting
    {
        [SerializeField] [InlineEditor] private List<CurrencyData> _currencies = new();

        public IReadOnlyList<CurrencyData> Currencies => _currencies;

        public CurrencyData GetCurrency(string type)
        {
            foreach (var def in _currencies)
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
                list: _currencies,
                getId: a => a.Id,
                generatedClassName: "CurrencyType",
                logTag: "CurrencyService"
            );
        }
#endif
    }
}
