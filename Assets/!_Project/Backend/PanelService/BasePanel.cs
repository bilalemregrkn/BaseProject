using Cysharp.Threading.Tasks;
using PrimeTween;
using Sirenix.OdinInspector;
using Tools.SmartComponent;
using UnityEngine;

namespace Plugins.PanelService
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BasePanel : SmartComponent, IPanel
    {
        [SerializeField] private bool customId;

        [ShowIf(nameof(customId))] [SerializeField]
        private string _id;

        [SerializeField] private float _fadeDuration = 0.25f;

        private CanvasGroup _canvasGroup;
        private bool _isVisible;

        public string Id => customId ? _id : transform.name;
        public bool IsVisible => _isVisible;

        protected virtual void Awake()
        {
            _canvasGroup = Get<CanvasGroup>(Source.Self);
            SetVisible(false, instant: true);
        }

        public async UniTask ShowAsync()
        {
            if (_isVisible) return;
            _isVisible = true;

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = true;

            await Tween.Alpha(_canvasGroup, 1f, _fadeDuration);

            _canvasGroup.interactable = true;
            OnShown();
        }

        public async UniTask HideAsync()
        {
            if (!_isVisible) return;
            _isVisible = false;

            _canvasGroup.interactable = false;

            await Tween.Alpha(_canvasGroup, 0f, _fadeDuration);

            _canvasGroup.blocksRaycasts = false;
            OnHidden();
        }

        private void SetVisible(bool visible, bool instant)
        {
            _isVisible = visible;
            _canvasGroup.alpha = visible ? 1f : 0f;
            _canvasGroup.interactable = visible;
            _canvasGroup.blocksRaycasts = visible;
        }

        protected virtual void OnShown()
        {
        }

        protected virtual void OnHidden()
        {
        }
    }
}