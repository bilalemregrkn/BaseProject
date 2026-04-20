using Reflex.Core;
using Reflex.Enums;
using UnityEngine;

namespace Plugins.CurrencyService
{
    public class CurrencyServiceInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private CurrencySettings _settings;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings);
            builder.RegisterType(typeof(CurrencyVault),    new[] { typeof(CurrencyVault) },    Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(CurrencyService),  new[] { typeof(ICurrencyService) }, Lifetime.Singleton, Resolution.Lazy);
        }
    }
}
