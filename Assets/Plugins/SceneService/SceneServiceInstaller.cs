using Reflex.Core;
using UnityEngine;

namespace Plugins.SceneService
{
    public class SceneServiceInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterFactory<ISceneService>(container =>
            {
                var go = new GameObject("SceneService");
                DontDestroyOnLoad(go);
                var service = go.AddComponent<SceneService>();
                service.Init(container.Single<Plugins.EventBus.IEventBus>());
                return service;
            }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
        }
    }
}
