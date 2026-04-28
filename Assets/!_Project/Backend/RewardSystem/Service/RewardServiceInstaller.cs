using Reflex.Core;
using Reflex.Enums;
using UnityEngine;

namespace Backend.Systems.Reward
{
    public class RewardServiceInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private RewardSettings _settings;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(_settings);
            builder.RegisterType(typeof(RewardVault),   new[] { typeof(RewardVault) },   Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
            builder.RegisterType(typeof(RewardService), new[] { typeof(IRewardService) }, Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
        }
    }
}
