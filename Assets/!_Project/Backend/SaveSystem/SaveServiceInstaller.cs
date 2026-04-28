using Reflex.Core;
using UnityEngine;

namespace Backend.Systems.Save
{
    public class SaveServiceInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(SaveService), new[] { typeof(ISaveService) }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
        }
    }
}
