using Reflex.Core;
using UnityEngine;

namespace Plugins.HapticService
{
    public class HapticServiceInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(HapticService), new[] { typeof(IHapticService) }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
        }
    }
}
