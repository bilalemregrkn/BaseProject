using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Backend.Systems.Audio
{
    public class AudioServiceInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private AudioSettings _settings;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(_settings);
            builder.RegisterType(typeof(AudioVault),  new[] { typeof(AudioVault) },   Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterFactory<AudioPlayer>(container =>
            {
                var go = new GameObject("AudioPlayer");
                DontDestroyOnLoad(go);
                return go.AddComponent<AudioPlayer>();
            }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(AudioService), new[] { typeof(IAudioService) }, Lifetime.Singleton, Resolution.Lazy);
        }
    }
}
