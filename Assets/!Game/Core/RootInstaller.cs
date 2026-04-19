using Plugins.AudioService;
using Plugins.EventBus;
using Plugins.HapticService;
using Plugins.MusicService;
using Plugins.PanelService;
using Plugins.SaveService;
using Plugins.UpdateService;
using Reflex.Core;
using UnityEngine;

public class RootInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder builder)
    {
        builder.RegisterValue(new EventBus(), new[] { typeof(IEventBus) });

        var updateService = new GameObject("UpdateService").AddComponent<UpdateService>();
        DontDestroyOnLoad(updateService);
        builder.RegisterValue(updateService, new[] { typeof(IUpdateService) });

        builder.RegisterType(typeof(SaveService), new[] { typeof(ISaveService) }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);

        builder.RegisterFactory<IAudioService>(container =>
        {
            var go = new GameObject("AudioService");
            DontDestroyOnLoad(go);
            var service = go.AddComponent<AudioService>();
            service.Init(container.Single<ISaveService>());
            return service;
        }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);

        builder.RegisterFactory<IMusicService>(container =>
        {
            var go = new GameObject("MusicService");
            DontDestroyOnLoad(go);
            var service = go.AddComponent<MusicService>();
            service.Init(container.Single<ISaveService>());
            return service;
        }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);

        builder.RegisterType(typeof(HapticService), new[] { typeof(IHapticService) }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);

        builder.RegisterType(typeof(PanelService), new[] { typeof(IPanelService) }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
    }
}