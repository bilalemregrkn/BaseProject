using Plugins.EventBus;

namespace Plugins.CurrencyService
{
    public struct CurrencyChanged : IEvent
    {
        public CurrencyType Type;
        public int OldAmount;
        public int NewAmount;
    }

    public struct CurrencyInsufficient : IEvent
    {
        public CurrencyType Type;
        public int Required;
        public int Available;
    }
}
