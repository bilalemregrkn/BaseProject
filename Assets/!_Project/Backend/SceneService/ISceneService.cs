using Cysharp.Threading.Tasks;
using Plexitugins.SceneService;

namespace Plugins.SceneService
{
    public interface ISceneService
    {
        bool IsLoading { get; }
        string ActiveScene { get; }

        UniTask LoadAsync(string id, LoadMode mode = LoadMode.Single);
        UniTask LoadAsync(BaseScene scene, LoadMode mode = LoadMode.Single);
        UniTask UnloadAsync(string id);
        UniTask UnloadAsync(BaseScene scene);
        bool IsLoaded(string id);
    }

    public enum LoadMode
    {
        Single,
        Additive
    }
}
