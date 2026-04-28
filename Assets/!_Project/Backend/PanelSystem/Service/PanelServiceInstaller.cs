using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Backend.Systems.Panel
{
    public class PanelServiceInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private PanelServiceSettings _settings;

        public void InstallBindings(ContainerBuilder builder)
        {
            var settings = _settings;
            builder.RegisterFactory<IPanelService>(
                c => new PanelService(settings, c),
                Lifetime.Singleton,
                Resolution.Lazy
            );
        }
    }
}
