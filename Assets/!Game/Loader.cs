using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public string sceneName;

    private void Start()
    {
        Load();

        // LoadExtra();
    }

    private void Load()
    {
        // here we take boot scene container just for an example, you can use any container you need
        var bootSceneContainer = gameObject.scene.GetSceneContainer();
        void OverrideParent(Scene scene, ContainerBuilder builder) => builder.SetParent(bootSceneContainer);

        ContainerScope.OnSceneContainerBuilding += OverrideParent;

        // If you are loading scenes without addressables
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single).completed += operation =>
        {
            ContainerScope.OnSceneContainerBuilding -= OverrideParent;
        };

        // // If you are loading scenes with addressables
        // Addressables.LoadSceneAsync("Lobby", LoadSceneMode.Additive).Completed += operation =>
        // {
        //     ContainerScope.OnSceneContainerBuilding -= OverrideParent;
        // };
    }


    private void LoadExtra()
    {
        // This way you can access ContainerBuilder of the scene that is currently building
        ContainerScope.OnSceneContainerBuilding += InstallExtra;

        // If you are loading scenes without addressables
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName).completed += operation =>
        {
            ContainerScope.OnSceneContainerBuilding -= InstallExtra;
        };

        // // If you are loading scenes with addressables
        // UnityEngine.AddressableAssets.Addressables.LoadSceneAsync("Greet").Completed += operation =>
        // {
        //     ContainerScope.OnSceneContainerBuilding -= InstallExtra;
        // };
    }

    private void InstallExtra(UnityEngine.SceneManagement.Scene scene, ContainerBuilder builder)
    {
        builder.RegisterValue("of Developers");
    }
}