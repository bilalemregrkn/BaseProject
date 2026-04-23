using Reflex.Core;
using UnityEngine;

namespace Game.Example
{
    public class GameSceneInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            // Scene-local bindings go here.
            // All root services (CurrencyService, EventBus, etc.) are already
            // available from the RootScope, so nothing extra is needed for this example.
        }
    }
}
