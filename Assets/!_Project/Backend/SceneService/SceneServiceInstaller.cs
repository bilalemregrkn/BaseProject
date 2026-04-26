using Reflex.Core;
using UnityEngine;

namespace Plugins.SceneService
{
    public class SceneServiceInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private SceneSettings _settings;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterFactory<ISceneService>(container =>
            {
                var service = new SceneService();
                service.Init(container.Single<Plugins.EventBus.IEventBus>(), _settings);
                return service;
            }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
        }
    }
}
