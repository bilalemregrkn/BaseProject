using Reflex.Core;
using UnityEngine;

namespace Backend.Systems.EventBus
{
    public class EventBusInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(new EventBus(), new[] { typeof(IEventBus) });
        }
    }
}
