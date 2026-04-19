using Reflex.Core;
using Plugins.SaveService;
using UnityEngine;

namespace Plugins.MusicService
{
    public class MusicServiceInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterFactory<IMusicService>(container =>
            {
                var go = new GameObject("MusicService");
                DontDestroyOnLoad(go);
                var service = go.AddComponent<MusicService>();
                service.Init(container.Single<ISaveService>());
                return service;
            }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
        }
    }
}
