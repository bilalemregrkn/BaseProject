using Reflex.Core;
using Reflex.Enums;
using UnityEngine;

namespace Backend.Systems.Timer
{
    public class TimerServiceInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(Deadline), Reflex.Enums.Lifetime.Transient, Reflex.Enums.Resolution.Lazy);
        }
    }
}
