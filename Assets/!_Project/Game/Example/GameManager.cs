using Cysharp.Threading.Tasks;
using Backend.Systems.Currency;
using Backend.Systems.EventBus;
using Backend.Systems.Panel;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Example
{
    public enum GameState
    {
        Start,
        Working,
        Completed
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private CircleSpawner circleSpawner;

        [Inject] private IPanelService _panelService;
        [Inject] private ICurrencyService _currencyService;
        [Inject] private IEventBus _eventBus;

        private void Start()
        {
            _panelService.ShowAsync(PanelType.Panel_Home).Forget();
            _eventBus.Subscribe<GameStartEvent>(OnGameStart);
        }

        private void OnDestroy()
        {
            _eventBus.Unsubscribe<GameStartEvent>(OnGameStart);
        }

        private void OnGameStart(GameStartEvent evt)
        {
            circleSpawner.Play();
        }
    }
}