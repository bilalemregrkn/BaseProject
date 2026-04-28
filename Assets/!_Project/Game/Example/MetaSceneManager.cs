using Backend.Systems.Scene;
using Reflex.Attributes;
using Backend.Systems.Component;
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