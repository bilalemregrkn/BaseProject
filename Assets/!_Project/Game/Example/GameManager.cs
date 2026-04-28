using System;
using Cysharp.Threading.Tasks;
using Backend.Systems.Currency;
using Backend.Systems.Panel;
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
            _panelService.ShowAsync(PanelType.Panel_GamePlay).Forget();
        }
    }
}
