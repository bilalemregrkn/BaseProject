using System;
using Plugins.CurrencyService;
using PrimeTween;
using Tools.SmartComponent;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Example
{
    [RequireComponent(typeof(Button))]
    public class CircleButton : SmartComponent
    {
        private Button _button;
        private Action _onClicked;
        private float _lifetime;
        private float _elapsed;
        private bool _consumed;

        public RectTransform RectTransform => transform as RectTransform;

        private void Awake()
        {
            _button = Get<Button>(Source.Self);
            _button.onClick.AddListener(OnClicked);
        }

        public void Init(Action onClicked, float lifetime)
        {
            _onClicked = onClicked;
            _lifetime = lifetime;
        }

        private void Update()
        {
            if (_consumed) return;

            _elapsed += Time.deltaTime;
            if (_elapsed >= _lifetime)
                Expire();
        }

        private void OnClicked()
        {
            if (_consumed) return;
            _consumed = true;
            _button.interactable = false;
            _onClicked?.Invoke();
            Tween.Scale(transform, Vector3.one, Vector3.zero, 0.15f, Ease.InBack)
                .OnComplete(() => Destroy(gameObject));
        }

        private void Expire()
        {
            _consumed = true;
            _button.interactable = false;
            Tween.Scale(transform, Vector3.one, Vector3.zero, 0.2f, Ease.InBack)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}
