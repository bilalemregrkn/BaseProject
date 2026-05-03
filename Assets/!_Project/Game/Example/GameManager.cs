using System;
using Backend.Systems.Panel;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Example
{
    public class GameManager : MonoBehaviour
    {
        [Inject] private IPanelService _panelService;
        
        private void Start()
        {
            _panelService.ShowAsync(PanelType.GamePlay);
        }
    }
}