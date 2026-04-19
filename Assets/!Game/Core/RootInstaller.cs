using Plugins.AudioService;
using Plugins.EventBus;
using Plugins.HapticService;
using Plugins.MusicService;
using Plugins.PanelService;
using Plugins.SaveManagement;
using Plugins.UpdateManager;
using Reflex.Core;
using UnityEngine;

public class RootInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder builder)
    {
        builder.RegisterValue(new EventBus(), new[] { typeof(IEventBus) });

        var updateManager = new GameObject().AddComponent<UpdateManager>();
        DontDestroyOnLoad(updateManager);
        builder.RegisterValue(updateManager, new[] { typeof(IUpdateManager) });

        builder.RegisterType(typeof(SaveManager), new[] { typeof(ISaveManager) }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);

        builder.RegisterFactory<IAudioService>(container =>
        {
            var go = new GameObject("AudioService");
            DontDestroyOnLoad(go);
            var service = go.AddComponent<AudioService>();
            service.Init(container.Single<ISaveManager>());
            return service;
        }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);

        builder.RegisterFactory<IMusicService>(container =>
        {
            var go = new GameObject("MusicService");
            DontDestroyOnLoad(go);
            var service = go.AddComponent<MusicService>();
            service.Init(container.Single<ISaveManager>());
            return service;
        }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);

        builder.RegisterType(typeof(HapticService), new[] { typeof(IHapticService) }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);

        builder.RegisterType(typeof(PanelService), new[] { typeof(IPanelService) }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
    }
}