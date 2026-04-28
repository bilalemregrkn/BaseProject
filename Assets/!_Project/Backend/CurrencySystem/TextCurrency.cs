using Backend.Systems.EventBus;
using Reflex.Attributes;
using Backend.Systems.Component;
using UnityEngine;

namespace Backend.Systems.Currency
{
    public class TextCurrency : BaseText
    {
        [SerializeField] private CurrencyData currencyData;
        [SerializeField] private string _format = "{0}";

        [Inject] private IEventBus _eventBus;
        [Inject] private ICurrencyService _currencyService;

        private void Start()
        {
            _eventBus.Subscribe<CurrencyChanged>(OnCurrencyChanged);
            Refresh(_currencyService.Get(currencyData.Id));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _eventBus?.Unsubscribe<CurrencyChanged>(OnCurrencyChanged);
        }

        private void OnCurrencyChanged(CurrencyChanged e)
        {
            if (e.Type != currencyData.Id) return;
            Refresh(e.NewAmount);
        }

        private void Refresh(int amount)
        {
            SetText(string.Format(_format, amount));
        }
    }
}
