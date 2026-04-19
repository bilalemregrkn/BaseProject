using Reflex.Core;
using UnityEngine;

public class GameSceneInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder builder)
    {
        builder.RegisterValue("Game");
    }
}