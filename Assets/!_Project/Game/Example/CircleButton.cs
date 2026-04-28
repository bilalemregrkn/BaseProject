using System;
using Backend.Systems.Currency;
using PrimeTween;
using Backend.Systems.Component;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Example
{
    [RequireComponent(typeof(Button))]
    public class CircleButton : SmartComponent
    {
        private Button _button;
        private Action _onClicked;
        private bool _consumed;

        public RectTransform RectTransform => transform as RectTransform;

        private void Awake()
        {
            _button = Get<Button>(Source.Self);
            _button.onClick.AddListener(OnClicked);
        }

        public void Init(Action onClicked)
        {
            _onClicked = onClicked;
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
    }
}
