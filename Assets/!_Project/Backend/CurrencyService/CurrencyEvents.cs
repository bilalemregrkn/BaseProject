using Plugins.EventBus;

namespace Plugins.CurrencyService
{
    public struct CurrencyChanged : IEvent
    {
        public string Type;
        public int OldAmount;
        public int NewAmount;
    }

    public struct CurrencyInsufficient : IEvent
    {
        public string Type;
        public int Required;
        public int Available;
    }
}
