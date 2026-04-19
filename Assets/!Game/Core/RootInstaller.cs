using Plugins.EventBus;
using Plugins.SaveManagement;
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

        builder.RegisterType(typeof(SaveManager), new[] { typeof(ISaveManager) }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Lazy);
    }
}