using Reflex.Core;
using Reflex.Enums;
using UnityEngine;

namespace Backend.Systems.Haptic
{
    public class HapticServiceInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private HapticSettings _settings;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(_settings);
            builder.RegisterType(typeof(HapticVault),    new[] { typeof(HapticVault) },    Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
            builder.RegisterType(typeof(HapticService),  new[] { typeof(IHapticService) }, Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
        }
    }
}
