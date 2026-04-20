using Reflex.Core;
using Reflex.Enums;
using UnityEngine;

namespace Plugins.MusicService
{
    public class MusicServiceInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private MusicSettings _settings;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings);
            builder.RegisterType(typeof(MusicVault), new[] { typeof(MusicVault) }, Lifetime.Singleton, Resolution.Lazy);

            builder.RegisterFactory<IMusicService>(container =>
            {
                var go = new GameObject("MusicService");
                DontDestroyOnLoad(go);
                var service = go.AddComponent<MusicService>();
                service.Init(container.Single<MusicVault>(), container.Single<MusicSettings>());
                return service;
            }, Lifetime.Singleton, Resolution.Lazy);
        }
    }
}
