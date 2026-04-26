using System;
using Cysharp.Threading.Tasks;
using Plugins.CurrencyService;
using Plugins.PanelService;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Example
{
    public enum GameState
    {
        Start,
        Working,
        Completed
    }

    public class GameManager : MonoBehaviour
    {
        [Inject] private IPanelService _panelService;
        [Inject] private ICurrencyService _currencyService;

        private void Start()
        {
            _panelService.ShowAsync(PanelIds.Game).Forget();
        }
    }
}
