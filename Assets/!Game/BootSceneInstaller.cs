using Reflex.Core;
using UnityEngine;

public class BootSceneInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder builder)
    {
        builder.RegisterValue("Boot");
    }
}