using UnityEngine;

namespace Plexitugins.SceneService
{
    public abstract class BaseScene : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _sceneName;

#if UNITY_EDITOR
        [SerializeField] private UnityEditor.SceneAsset _sceneAsset;

        private void OnValidate()
        {
            if (_sceneAsset != null)
                _sceneName = _sceneAsset.name;
        }
#endif

        public string Id        => _id;
        public string SceneName => _sceneName;
    }
}
