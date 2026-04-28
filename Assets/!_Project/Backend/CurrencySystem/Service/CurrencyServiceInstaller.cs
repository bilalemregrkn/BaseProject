using Reflex.Core;
using Reflex.Enums;
using UnityEngine;

namespace Backend.Systems.Currency
{
    public class CurrencyServiceInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private CurrencySettings _settings;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(_settings);
            builder.RegisterType(typeof(CurrencyVault),    new[] { typeof(CurrencyVault) },    Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
            builder.RegisterType(typeof(CurrencyService),  new[] { typeof(ICurrencyService) }, Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
        }
    }
}
