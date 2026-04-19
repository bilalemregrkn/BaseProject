using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Plugins.PanelService
{
    public sealed class PanelService : IPanelService
    {
        private readonly Dictionary<string, IPanel> _panels = new(16);

        public void Register(string id, IPanel panel)
        {
            if (string.IsNullOrEmpty(id) || panel == null) return;
            _panels[id] = panel;
        }

        public void Unregister(string id)
        {
            if (id != null) _panels.Remove(id);
        }

        public UniTask ShowAsync(string id)
        {
            return _panels.TryGetValue(id, out var panel) ? panel.ShowAsync() : UniTask.CompletedTask;
        }

        public UniTask HideAsync(string id)
        {
            return _panels.TryGetValue(id, out var panel) ? panel.HideAsync() : UniTask.CompletedTask;
        }

        public async UniTask ShowExclusiveAsync(string id)
        {
            var hideTasks = new List<UniTask>(_panels.Count);

            foreach (var (key, panel) in _panels)
            {
                if (key != id && panel.IsVisible)
                    hideTasks.Add(panel.HideAsync());
            }

            await UniTask.WhenAll(hideTasks);

            if (_panels.TryGetValue(id, out var target))
                await target.ShowAsync();
        }

        public bool IsVisible(string id) =>
            _panels.TryGetValue(id, out var panel) && panel.IsVisible;
    }
}
