namespace Backend.Systems.Scene
{
    public readonly struct SceneLoadStarted
    {
        public readonly string SceneName;
        public readonly LoadMode Mode;

        public SceneLoadStarted(string sceneName, LoadMode mode)
        {
            SceneName = sceneName;
            Mode = mode;
        }
    }

    public readonly struct SceneLoadCompleted
    {
        public readonly string SceneName;

        public SceneLoadCompleted(string sceneName) => SceneName = sceneName;
    }

    public readonly struct SceneUnloadStarted
    {
        public readonly string SceneName;

        public SceneUnloadStarted(string sceneName) => SceneName = sceneName;
    }

    public readonly struct SceneUnloadCompleted
    {
        public readonly string SceneName;

        public SceneUnloadCompleted(string sceneName) => SceneName = sceneName;
    }
}
