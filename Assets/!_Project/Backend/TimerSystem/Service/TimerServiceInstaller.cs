using Reflex.Core;
using Reflex.Enums;
using UnityEngine;

namespace Backend.Systems.Timer
{
    public class TimerServiceInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(TimerVault),   new[] { typeof(TimerVault) },   Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
            builder.RegisterType(typeof(TimerService), new[] { typeof(ITimerService) }, Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
        }
    }
}
