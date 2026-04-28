using Reflex.Core;
using Reflex.Enums;
using UnityEngine;

namespace Backend.Systems.Music
{
    public class MusicServiceInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private MusicSettings _settings;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(_settings);
            builder.RegisterType(typeof(MusicVault), new[] { typeof(MusicVault) }, Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);

            builder.RegisterFactory<IMusicService>(container =>
            {
                var go = new GameObject("MusicService");
                DontDestroyOnLoad(go);
                var service = go.AddComponent<MusicService>();
                service.Init(container.Single<MusicVault>(), container.Single<MusicSettings>());
                return service;
            }, Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
        }
    }
}
