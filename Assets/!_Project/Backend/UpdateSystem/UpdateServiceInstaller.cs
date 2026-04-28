using Reflex.Core;
using UnityEngine;

namespace Plugins.UpdateService
{
    public class UpdateServiceInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            var go = new GameObject("UpdateService");
            DontDestroyOnLoad(go);
            var service = go.AddComponent<UpdateService>();
            builder.RegisterValue(service, new[] { typeof(IUpdateService) });
        }
    }
}
