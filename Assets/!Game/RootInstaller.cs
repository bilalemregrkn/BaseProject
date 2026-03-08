using Plugins.EventBus;
using Plugins.UpdateManager;
using Reflex.Core;
using UnityEngine;

public class RootInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder builder)
    {
        builder.RegisterValue(new EventBus(), new[] { typeof(IEventBus) });

        var updateManager = new GameObject().AddComponent<UpdateManager>();
        DontDestroyOnLoad(updateManager);
        builder.RegisterValue(updateManager, new[] { typeof(IUpdateManager) });
    }
}