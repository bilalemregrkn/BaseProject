using Plugins.CurrencyService;
using Plugins.EventBus;
using Plugins.PanelService;
using Plugins.SaveService;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace Game.Example
{
    public class PanelGamePlay : BasePanel
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _highScoreText;

        [Inject] private IEventBus _eventBus;
        [Inject] private ICurrencyService _currencyService;
        [Inject] private IPanelService _panelService;
        [Inject] private ISaveService _saveService;

        private const string HighScoreKey = "ClickGame_HighScore";
        private int _highScore;

        protected override void Awake()
        {
            base.Awake();
            _highScore = _saveService.Load(HighScoreKey, 0);
        }

        private void Start()
        {
            _panelService.Register(PanelIds.Game, this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _panelService?.Unregister(PanelIds.Game);
        }

        private void OnEnable()
        {
            _eventBus?.Subscribe<CurrencyChanged>(OnCurrencyChanged);
        }

        private void OnDisable()
        {
            _eventBus?.Unsubscribe<CurrencyChanged>(OnCurrencyChanged);
        }

        protected override void OnShown()
        {
            RefreshScore(0);
        }

        private void OnCurrencyChanged(CurrencyChanged e)
        {
            if (e.Type != CurrencyType.Gold) return;
            RefreshScore(e.NewAmount);
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
