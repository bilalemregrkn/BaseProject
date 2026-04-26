using Plugins.SceneService;
using Reflex.Attributes;
using Tools.SmartComponent;
using UnityEngine;

namespace Game.Example
{
    public class MetaSceneManager : MonoBehaviour
    {
        public BaseButton startButton;

        private ISceneService _sceneService;

        [Inject]
        private void Construct(ISceneService sceneService)
        {
            _sceneService = sceneService;
        }

        private void Awake()
        {
            startButton.MyButton.onClick.AddListener(OnStartButtonClicked);
        }

        private void OnStartButtonClicked()
        {
            _sceneService.LoadAsync(SceneType.Game);
        }
    }
}