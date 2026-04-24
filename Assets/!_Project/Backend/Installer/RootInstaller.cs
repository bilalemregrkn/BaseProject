using Plugins.AudioService;
using AudioSettings = Plugins.AudioService.AudioSettings;
using Plugins.CurrencyService;
using Plugins.EventBus;
using Plugins.PanelService;
using Plugins.SaveService;
using Plugins.UpdateService;
using Reflex.Core;
using Reflex.Enums;
using Resolution = Reflex.Enums.Resolution;
using UnityEngine;

public class RootInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private CurrencySettings _currencySettings;
    [SerializeField] private AudioSettings _audioSettings;

    public void InstallBindings(ContainerBuilder builder)
    {
        // EventBus — foundation, no dependencies
        builder.RegisterValue(new EventBus(), new[] { typeof(IEventBus) });

        // SaveService
        builder.RegisterType(typeof(SaveService), new[] { typeof(ISaveService) },
            Lifetime.Singleton, Resolution.Lazy);

        // UpdateService — needs a persistent GameObject
        builder.RegisterFactory<IUpdateService>(_ =>
        {
            var go = new GameObject("UpdateService");
            DontDestroyOnLoad(go);
            return go.AddComponent<UpdateService>();
        }, Lifetime.Singleton, Resolution.Lazy);

        // PanelService
        builder.RegisterType(typeof(PanelService), new[] { typeof(IPanelService) },
            Lifetime.Singleton, Resolution.Lazy);

        // CurrencyService
        builder.RegisterValue(_currencySettings);
        builder.RegisterType(typeof(CurrencyVault),   new[] { typeof(CurrencyVault) },   Lifetime.Singleton, Resolution.Lazy);
        builder.RegisterType(typeof(CurrencyService), new[] { typeof(ICurrencyService) }, Lifetime.Singleton, Resolution.Lazy);

        // AudioService
        builder.RegisterValue(_audioSettings);
        builder.RegisterType(typeof(AudioVault),   new[] { typeof(AudioVault) },   Lifetime.Singleton, Resolution.Lazy);
        builder.RegisterFactory<AudioPlayer>(_ =>
        {
            var go = new GameObject("AudioPlayer");
            DontDestroyOnLoad(go);
            return go.AddComponent<AudioPlayer>();
        }, Lifetime.Singleton, Resolution.Lazy);
        builder.RegisterType(typeof(AudioService), new[] { typeof(IAudioService) }, Lifetime.Singleton, Resolution.Lazy);
    }
}
