using Reflex.Core;
using Reflex.Enums;
using UnityEngine;

namespace Plugins.HapticService
{
    public class HapticServiceInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private HapticSettings _settings;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings);
            builder.RegisterType(typeof(HapticVault),    new[] { typeof(HapticVault) },    Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(HapticService),  new[] { typeof(IHapticService) }, Lifetime.Singleton, Resolution.Lazy);
        }
    }
}
