using Backend.Systems.EventBus;
using Backend.Systems.Panel;
using Backend.Systems.Save;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace Game.Example
{
    public class GamePlay : PanelBase
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _highScoreText;

        [Inject] private IEventBus _eventBus;
        [Inject] private IPanelService _panelService;
        [Inject] private ISaveService _saveService;

        private const string HighScoreKey = "ClickGame_HighScore";
        private int _highScore;

        private void Start()
        {
            _highScore = _saveService.Load(HighScoreKey, 0);
            _panelService.Register(PanelType.Panel_GamePlay, this);
            _eventBus.Subscribe<CircleClickedEvent>(OnCircleClicked);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _panelService?.Unregister(PanelType.Panel_GamePlay);
            _eventBus?.Unsubscribe<CircleClickedEvent>(OnCircleClicked);
        }

        protected override void OnShown()
        {
            RefreshScore(0);
        }

        private void OnCircleClicked(CircleClickedEvent e)
        {
            RefreshScore(e.Score);
        }

        private void RefreshScore(int score)
        {
            _scoreText.text = $"Score: {score}";

            if (score > _highScore)
            {
                _highScore = score;
                _saveService.Save(HighScoreKey, _highScore);
            }

            _highScoreText.text = $"Best: {_highScore}";
        }
    }
}
