using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Backend.Systems.EventBus;
using UnityEngine;

namespace Backend.Systems.Scene
{
    public sealed class SceneService : ISceneService
    {
        private readonly IEventBus _eventBus;
        private readonly SceneSettings _settings;
        private readonly HashSet<string> _loadedScenes = new(8);
        private bool _isLoading;

        public bool IsLoading => _isLoading;
        public string ActiveScene => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        public SceneService(IEventBus eventBus, SceneSettings settings)
        {
            _eventBus = eventBus;
            _settings = settings;

            var count = UnityEngine.SceneManagement.SceneManager.sceneCount;
            for (int i = 0; i < count; i++)
                _loadedScenes.Add(UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).name);
        }

        public UniTask LoadAsync(SceneData data, LoadMode mode = LoadMode.Single)
            => LoadAsyncInternal(data.ToConfig().SceneName, mode);

        public UniTask LoadAsync(string id, LoadMode mode = LoadMode.Single)
        {
            var data = _settings.GetData(id);
            if (data == null)
            {
                Debug.LogError($"[SceneService] No SceneData found for id '{id}'");
                return UniTask.CompletedTask;
            }
            return LoadAsyncInternal(data.ToConfig().SceneName, mode);
        }

        public UniTask UnloadAsync(SceneData data)
            => UnloadAsyncInternal(data.ToConfig().SceneName);

        public UniTask UnloadAsync(string id)
        {
            var data = _settings.GetData(id);
            if (data == null)
            {
                Debug.LogError($"[SceneService] No SceneData found for id '{id}'");
                return UniTask.CompletedTask;
            }
            return UnloadAsyncInternal(data.ToConfig().SceneName);
        }

        public bool IsLoaded(string id)
        {
            var data = _settings.GetData(id);
            return data != null && _loadedScenes.Contains(data.ToConfig().SceneName);
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
