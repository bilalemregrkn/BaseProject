using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Plugins.EventBus;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.SceneManagement
{
    public sealed class SceneManager : MonoBehaviour, ISceneManager
    {
        private IEventBus _eventBus;
        private readonly HashSet<string> _loadedScenes = new(8);
        private bool _isLoading;

        public bool IsLoading => _isLoading;
        public string ActiveScene => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        public void Init(IEventBus eventBus)
        {
            _eventBus = eventBus;

            var count = UnityEngine.SceneManagement.SceneManager.sceneCount;
            for (int i = 0; i < count; i++)
                _loadedScenes.Add(UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).name);
        }

        public async UniTask LoadAsync(string sceneName, LoadMode mode = LoadMode.Single)
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

        public async UniTask UnloadAsync(string sceneName)
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

        public bool IsLoaded(string sceneName) => _loadedScenes.Contains(sceneName);
    }
}
