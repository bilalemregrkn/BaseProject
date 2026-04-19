using Reflex.Core;
using Plugins.SaveService;
using UnityEngine;

namespace Plugins.AudioService
{
    public class AudioServiceInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            Debug.Log("Installing AudioService bindings...");
            
            builder.RegisterFactory<IAudioService>(container =>
            {
                var go = new GameObject("AudioService");
                DontDestroyOnLoad(go);
                var service = go.AddComponent<AudioService>();
                service.Init(container.Single<ISaveService>());
                return service;
            }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
        }
    }
}
