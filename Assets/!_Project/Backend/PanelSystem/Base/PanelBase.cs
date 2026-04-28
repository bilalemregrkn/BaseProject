using Cysharp.Threading.Tasks;
using PrimeTween;
using Sirenix.OdinInspector;
using Backend.Systems.Component;
using UnityEngine;

namespace Backend.Systems.Panel
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class PanelBase : SmartComponent, IPanel
    {
        private CanvasGroup CanvasGroup => Get<CanvasGroup>(Source.Self);
        private bool _isVisible;
        private float _fadeDuration = 0.125f;

        public bool IsVisible => _isVisible;

        protected virtual void Awake()
        {
            SetVisible(false, instant: true);
        }

        public async UniTask ShowAsync()
        {
            if (_isVisible) return;
            _isVisible = true;

            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = true;

            await Tween.Alpha(CanvasGroup, 1f, _fadeDuration);

            CanvasGroup.interactable = true;
            OnShown();
        }

        public async UniTask HideAsync()
        {
            if (!_isVisible) return;
            _isVisible = false;

            CanvasGroup.interactable = false;

            await Tween.Alpha(CanvasGroup, 0f, _fadeDuration);

            CanvasGroup.blocksRaycasts = false;
            OnHidden();
        }

        private void SetVisible(bool visible, bool instant)
        {
            _isVisible = visible;
            CanvasGroup.alpha = visible ? 1f : 0f;
            CanvasGroup.interactable = visible;
            CanvasGroup.blocksRaycasts = visible;
        }

        protected virtual void OnShown()
        {
        }

        protected virtual void OnHidden()
        {
        }
    }
}