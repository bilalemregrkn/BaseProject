using Cysharp.Threading.Tasks;

namespace Plugins.SceneService
{
    public interface ISceneService
    {
        bool IsLoading { get; }
        string ActiveScene { get; }

        UniTask LoadAsync(string id, LoadMode mode = LoadMode.Single);
        UniTask LoadAsync(SceneData data, LoadMode mode = LoadMode.Single);
        UniTask UnloadAsync(string id);
        UniTask UnloadAsync(SceneData data);
        bool IsLoaded(string id);
    }

    public enum LoadMode
    {
        Single,
        Additive
    }
}
