using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using Tools.SmartComponent;
using UnityEngine;

namespace Plugins.PanelService
{
    public class PanelHome : BasePanel
    {
        [SerializeField] private BaseButton startButton;

        [Inject] private IPanelService _panelService;

        private void Start()
        {
            _panelService.Register(PanelType.Home, this);
            startButton.Clicked += OnStartClicked;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _panelService?.Unregister(PanelType.Home);
        }

        private void OnStartClicked()
        {
            _panelService.ShowExclusiveAsync(PanelType.Game).Forget();
        }
    }
}
