using Plugins.SceneService;
using Reflex.Attributes;
using Tools.SmartComponent;
using UnityEngine;

namespace Game.Example
{
    public class MetaSceneManager : MonoBehaviour
    {
        public BaseButton startButton;

        [Inject] private ISceneService _sceneService;

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