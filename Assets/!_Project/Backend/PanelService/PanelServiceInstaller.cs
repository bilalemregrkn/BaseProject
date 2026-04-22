using Reflex.Core;
using UnityEngine;

namespace Plugins.PanelService
{
    public class PanelServiceInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(PanelService), new[] { typeof(IPanelService) }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
        }
    }
}
