using Plugins.EventBus;
using Reflex.Attributes;
using Tools.SmartComponent;
using UnityEngine;

namespace Plugins.CurrencyService
{
    public class TextCurrency : BaseText
    {
        [SerializeField] private BaseCurrency _currency;
        [SerializeField] private string _format = "{0}";

        [Inject] private IEventBus _eventBus;
        [Inject] private ICurrencyService _currencyService;

        private void Start()
        {
            _eventBus.Subscribe<CurrencyChanged>(OnCurrencyChanged);
            Refresh(_currencyService.Get(_currency.Id));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _eventBus?.Unsubscribe<CurrencyChanged>(OnCurrencyChanged);
        }

        private void OnCurrencyChanged(CurrencyChanged e)
        {
            if (e.Type != _currency.Id) return;
            Refresh(e.NewAmount);
        }

        private void Refresh(int amount)
        {
            SetText(string.Format(_format, amount));
        }
    }
}
