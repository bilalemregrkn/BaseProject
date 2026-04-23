namespace Plugins.CurrencyService
{
    public interface ICurrencyService
    {
        int Get(string type);
        void Add(string type, int amount);
        bool TrySpend(string type, int amount);
        bool Has(string type, int amount);
        void Set(string type, int amount);
    }
}
