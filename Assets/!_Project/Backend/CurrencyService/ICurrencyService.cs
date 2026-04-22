namespace Plugins.CurrencyService
{
    public interface ICurrencyService
    {
        int Get(CurrencyType type);
        void Add(CurrencyType type, int amount);
        bool TrySpend(CurrencyType type, int amount);
        bool Has(CurrencyType type, int amount);
        void Set(CurrencyType type, int amount);
    }
}
