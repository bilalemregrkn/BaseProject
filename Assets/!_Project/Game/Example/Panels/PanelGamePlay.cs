using System;
using Backend.Systems.Audio;
using Backend.Systems.Component;
using Backend.Systems.Currency;
using Backend.Systems.EventBus;
using Backend.Systems.Panel;
using Backend.Systems.Save;
using Backend.Systems.Timer;
using Backend.Systems.Update;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using AudioType = Backend.Systems.Audio.AudioType;


namespace Game.Example
{
    public class PanelGamePlay : PanelBase, IUpdatable
    {
        private const string BestScoreKey = "best_score";
        private const int ScoreIncrement = 10;
        private const int GoldIncrement = 5;

        private int _score;

        [SerializeField] private BaseButton buttonOpenSetting;
        [SerializeField] private BaseButton buttonGiveScore;
        [SerializeField] private BaseButton buttonGiveGold;
        [SerializeField] private BaseButton buttonReset;
        [SerializeField] private BaseButton buttonTimer;
        [SerializeField] private TextMeshProUGUI textScore;
        [SerializeField] private TextMeshProUGUI textBestScore;
        [SerializeField] private TextMeshProUGUI textTimer;

        [Inject] private IPanelService _panelService;
        [Inject] private IEventBus _eventBus;
        [Inject] private ISaveService _saveService;
        [Inject] private ICurrencyService _currencyService;
        [Inject] private IAudioService _audioService;
        [Inject] private ITimerService _timerService;

        private void Start()
        {
            buttonOpenSetting.MyButton.onClick.AddListener(OnClickOpenSetting);
            buttonGiveScore.MyButton.onClick.AddListener(OnClickGiveScore);
            buttonGiveGold.MyButton.onClick.AddListener(OnClickGiveGold);
            buttonReset.MyButton.onClick.AddListener(OnClickReset);
            buttonTimer.MyButton.onClick.AddListener(OnClickTimer);

            RefreshScoreText();
        }

        private void OnClickTimer()
        {
            _audioService.Play(AudioType.Button_press);
            _timerService.Start("ExampleTimer", TimeSpan.FromSeconds(5));
            _eventBus.Subscribe<TimerExpired>(OnTimerExpired);
        }

        private void OnTimerExpired(TimerExpired obj)
        {
            if (obj.Id != "ExampleTimer")
                return;
            _eventBus.Unsubscribe<TimerExpired>(OnTimerExpired);
            textTimer.text = "Timer: 0.0s";
        }

        private void OnClickReset()
        {
            _audioService.Play(AudioType.Button_press);
            _saveService.Save(BestScoreKey, 0);
            _score = 0;
            _currencyService.Set(CurrencyType.Gold, 0);
            RefreshScoreText();
        }

        protected override void OnShown()
        {
            RefreshScoreText();
        }

        private void OnClickOpenSetting()
        {
            _audioService.Play(AudioType.Beep);
            _panelService.ShowAsync(PanelType.Setting).Forget();
        }

        private void OnClickGiveScore()
        {
            _audioService.Play(AudioType.Button_press);
            _score += ScoreIncrement;
            RefreshScoreText();

            CheckBestScore();
        }

        private void CheckBestScore()
        {
            var best = _saveService.Load(BestScoreKey, 0);
            if (_score <= best) return;
            _saveService.Save(BestScoreKey, _score);
            textBestScore.text = $"Best: {_score}";
        }

        private void OnClickGiveGold()
        {
            _audioService.Play(AudioType.Button_press);
            _currencyService.Add(CurrencyType.Gold, GoldIncrement);
        }

        private void RefreshScoreText()
        {
            textScore.text = $"Score: {_score}";
            textBestScore.text = $"Best: {_saveService.Load(BestScoreKey, 0)}";
        }

        public void Tick(float dt)
        {
            if (_timerService.IsRunning("ExampleTimer"))
                textTimer.text = $"Timer: {_timerService.GetRemaining("ExampleTimer").TotalSeconds:F1}s";
        }
    }
}