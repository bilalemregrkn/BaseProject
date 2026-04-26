using UnityEngine;

namespace Plexitugins.SceneService
{
    public abstract class BaseScene : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _sceneName;

        public string Id        => _id;
        public string SceneName => _sceneName;
    }
}
