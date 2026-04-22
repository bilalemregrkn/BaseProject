using Cysharp.Threading.Tasks;

namespace Plugins.SceneService
{
    public interface ISceneService
    {
        bool IsLoading { get; }
        string ActiveScene { get; }

        UniTask LoadAsync(string sceneName, LoadMode mode = LoadMode.Single);
        UniTask UnloadAsync(string sceneName);
        bool IsLoaded(string sceneName);
    }

    public enum LoadMode
    {
        Single,
        Additive
    }
}
