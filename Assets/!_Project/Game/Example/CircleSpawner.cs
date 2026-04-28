using Backend.Systems.Audio;
using Backend.Systems.Currency;
using Backend.Systems.EventBus;
using Backend.Systems.Update;
using PrimeTween;
using Reflex.Attributes;
using UnityEngine;
using AudioType = Backend.Systems.Audio.AudioType;


namespace Game.Example
{
    public class CircleSpawner : MonoBehaviour, IUpdatable
    {
        [SerializeField] private CircleButton _circlePrefab;
        [SerializeField] private RectTransform _spawnArea;
        [SerializeField] private float _spawnInterval = 1.5f;

        [Inject] private IUpdateService _updateService;
        [Inject] private ICurrencyService _currencyService;
        [Inject] private IAudioService _audioService;
        [Inject] private IEventBus _eventBus;

        private float _timer;
        private bool _active;

        private void OnDestroy()
        {
            _updateService?.Remove(this);
        }

        public void Tick(float dt)
        {
            if (!_active) return;

            _timer += dt;
            if (_timer >= _spawnInterval)
            {
                _timer = 0f;
                SpawnCircle();
            }
        }

        private void SpawnCircle()
        {
            var circle = Instantiate(_circlePrefab, _spawnArea);
            circle.Init(OnCircleClicked);

            var rect = _spawnArea.rect;
            circle.RectTransform.anchoredPosition = new Vector2(
                Random.Range(rect.xMin + 50f, rect.xMax - 50f),
                Random.Range(rect.yMin + 50f, rect.yMax - 50f)
            );

            Tween.Scale(circle.transform, Vector3.zero, Vector3.one, 0.2f, Ease.OutBack);
        }

        private void OnCircleClicked()
        {
            //Action
            _audioService.Play(AudioType.Beep);
            _currencyService.Add(CurrencyType.Gold, 1);
            _eventBus.Publish(new CircleClickedEvent());
            _timer = _spawnInterval;
        }

        public void Play()
        {
            _timer = 0f;
            _active = true;
            _updateService.Add(this);
        }

        public void Stop()
        {
            _active = false;
            _updateService.Remove(this);
        }
    }
}
