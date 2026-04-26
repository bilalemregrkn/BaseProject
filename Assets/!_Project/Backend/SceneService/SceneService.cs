using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Plugins.EventBus;
using Plexitugins.SceneService;
using UnityEngine;

namespace Plugins.SceneService
{
    public sealed class SceneService : ISceneService
    {
        private IEventBus _eventBus;
        private SceneSettings _settings;
        private readonly HashSet<string> _loadedScenes = new(8);
        private bool _isLoading;

        public bool IsLoading => _isLoading;
        public string ActiveScene => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        public void Init(IEventBus eventBus, SceneSettings settings)
        {
            _eventBus = eventBus;
            _settings = settings;

            var count = UnityEngine.SceneManagement.SceneManager.sceneCount;
            for (int i = 0; i < count; i++)
                _loadedScenes.Add(UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).name);
        }

        public UniTask LoadAsync(BaseScene scene, LoadMode mode = LoadMode.Single)
            => LoadAsyncInternal(scene.SceneName, mode);

        public UniTask LoadAsync(string id, LoadMode mode = LoadMode.Single)
        {
            var scene = _settings.GetScene(id);
            if (scene == null)
            {
                Debug.LogError($"[SceneService] No BaseScene found for id '{id}'");
                return UniTask.CompletedTask;
            }
            return LoadAsyncInternal(scene.SceneName, mode);
        }

        public UniTask UnloadAsync(BaseScene scene)
            => UnloadAsyncInternal(scene.SceneName);

        public UniTask UnloadAsync(string id)
        {
            var scene = _settings.GetScene(id);
            if (scene == null)
            {
                Debug.LogError($"[SceneService] No BaseScene found for id '{id}'");
                return UniTask.CompletedTask;
            }
            return UnloadAsyncInternal(scene.SceneName);
        }

        public bool IsLoaded(string id)
        {
            var scene = _settings.GetScene(id);
            return scene != null && _loadedScenes.Contains(scene.SceneName);
        }

        private async UniTask LoadAsyncInternal(string sceneName, LoadMode mode)
        {
            if (string.IsNullOrEmpty(sceneName) || _isLoading) return;
            if (mode == LoadMode.Additive && _loadedScenes.Contains(sceneName)) return;

            _isLoading = true;
            _eventBus?.Publish(new SceneLoadStarted(sceneName, mode));

            var loadMode = mode == LoadMode.Additive
                ? UnityEngine.SceneManagement.LoadSceneMode.Additive
                : UnityEngine.SceneManagement.LoadSceneMode.Single;

            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, loadMode);

            if (mode == LoadMode.Single)
                _loadedScenes.Clear();

            _loadedScenes.Add(sceneName);
            _isLoading = false;

            _eventBus?.Publish(new SceneLoadCompleted(sceneName));
        }

        private async UniTask UnloadAsyncInternal(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName) || _isLoading) return;
            if (!_loadedScenes.Contains(sceneName)) return;

            _isLoading = true;
            _eventBus?.Publish(new SceneUnloadStarted(sceneName));

            await UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);

            _loadedScenes.Remove(sceneName);
            _isLoading = false;

            _eventBus?.Publish(new SceneUnloadCompleted(sceneName));
        }
    }
}
