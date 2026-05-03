using System;
using Backend.Systems.Component;
using Backend.Systems.EventBus;
using Backend.Systems.Panel;
using Backend.Systems.Save;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace Game.Example
{
    public class PanelGamePlay : PanelBase
    {
        [SerializeField] private BaseButton buttonOpenSetting;
        [SerializeField] private BaseButton buttonGiveScore;
        [SerializeField] private BaseButton buttonGiveGold;
        [SerializeField] private TextMeshProUGUI textScore;
        
        [Inject] private IPanelService _panelService;
        [Inject] private IEventBus _eventBus;
        [Inject] private ISaveService _saveService;

        private void Start()
        {
            buttonOpenSetting.MyButton.onClick.AddListener(OnClickOpenSetting);
            buttonGiveScore.MyButton.onClick.AddListener(OnClickGiveScore);
            buttonGiveGold.MyButton.onClick.AddListener(OnClickGiveGold);
        }
    }
}
