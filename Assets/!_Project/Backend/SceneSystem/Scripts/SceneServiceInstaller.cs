using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Backend.Systems.Scene
{
    public class SceneServiceInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private SceneSettings _settings;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(_settings);
            builder.RegisterType(typeof(SceneService), new[] { typeof(ISceneService) }, Lifetime.Singleton, Resolution.Lazy);
        }
    }
}
