using Plugins.CurrencyService;
using Plugins.EventBus;
using Plugins.PanelService;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace Game.Example
{
    public class GamePanel : BasePanel
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _highScoreText;

        [Inject] private IEventBus _eventBus;
        [Inject] private ICurrencyService _currencyService;
        [Inject] private IPanelService _panelService;

        private int _highScore;

        protected override void Awake()
        {
            base.Awake();
            _highScore = PlayerPrefs.GetInt("ClickGame_HighScore", 0);
        }

        private void Start()
        {
            // Injection is complete by Start; safe to use injected fields here
            _panelService.Register(PanelIds.Game, this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _panelService?.Unregister(PanelIds.Game);
        }

        private void OnEnable()
        {
            // Guard: OnEnable fires during base.Awake() before injection is done
            _eventBus?.Subscribe<CurrencyChanged>(OnCurrencyChanged);
        }

        private void OnDisable()
        {
            _eventBus?.Unsubscribe<CurrencyChanged>(OnCurrencyChanged);
        }

        protected override void OnShown()
        {
            RefreshScore(_currencyService.Get(CurrencyType.Gold));
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
                PlayerPrefs.SetInt("ClickGame_HighScore", _highScore);
            }

            _highScoreText.text = $"Best: {_highScore}";
        }
    }
}
