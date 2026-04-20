using Reflex.Core;
using Reflex.Enums;
using UnityEngine;

namespace Plugins.AudioService
{
    public class AudioServiceInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private AudioSettings _settings;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings);
            builder.RegisterType(typeof(AudioVault), new[] { typeof(AudioVault) }, Lifetime.Singleton, Resolution.Lazy);

            builder.RegisterFactory<IAudioService>(container =>
            {
                var go = new GameObject("AudioService");
                DontDestroyOnLoad(go);
                var service = go.AddComponent<AudioService>();
                service.Init(container.Single<AudioVault>());
                return service;
            }, Lifetime.Singleton, Resolution.Lazy);
        }
    }
}
