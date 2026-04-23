using Cysharp.Threading.Tasks;
using Plugins.CurrencyService;
using Plugins.PanelService;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Example
{
    public class GameEntryPoint : MonoBehaviour
    {
        [Inject] private IPanelService _panelService;
        [Inject] private ICurrencyService _currencyService;

        private void Start()
        {
            _currencyService.Set(CurrencyType.Gold, 0);
            _panelService.ShowAsync(PanelIds.Game).Forget();
        }
    }
}
