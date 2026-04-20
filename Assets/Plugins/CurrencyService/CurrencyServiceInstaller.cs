using Reflex.Core;
using Reflex.Enums;
using UnityEngine;

namespace Plugins.CurrencyService
{
    public class CurrencyServiceInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(CurrencyService), new[] { typeof(ICurrencyService) }, Lifetime.Singleton, Resolution.Lazy);
        }
    }
}
