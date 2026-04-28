using Backend.Systems.Component;
using Reflex.Attributes;
using UnityEngine;

namespace Backend.Systems.Panel
{
    public class PanelHome : PanelBase
    {
        [SerializeField] private BaseButton startButton;
        
        [Inject] IPanelService _panelService;

        protected override void Awake()
        {
            base.Awake();
            
            startButton.MyButton.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _panelService.HideAsync(PanelType.Panel_Home);
            _panelService.ShowAsync(PanelType.Panel_GamePlay);
            //todo fire signal
        }
    }
}