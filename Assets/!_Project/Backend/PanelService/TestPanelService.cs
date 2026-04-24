using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.PanelService
{
    public class TestPanelService : MonoBehaviour
    {
        [Inject] private IPanelService _service;

        public string id;

        [Button]
        public void Show()
        {
            _service.ShowAsync(id).Forget();
        }

        [Button]
        public void Hide()
        {
            _service.HideAsync(id).Forget();
        }
    }
}