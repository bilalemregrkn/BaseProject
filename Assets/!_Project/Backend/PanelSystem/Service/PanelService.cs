using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

namespace Backend.Systems.Panel
{
    public sealed class PanelService : IPanelService
    {
        private readonly Dictionary<string, IPanel> _panels = new(16);
        private readonly PanelServiceSettings _settings;
        private readonly Container _container;

        public PanelService(PanelServiceSettings settings, Container container)
        {
            _settings = settings;
            _container = container;
        }

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
            return GetOrCreate(id) is { } panel ? panel.ShowAsync() : UniTask.CompletedTask;
        }

        public UniTask HideAsync(string id)
        {
            return GetOrCreate(id) is { } panel ? panel.HideAsync() : UniTask.CompletedTask;
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

            if (GetOrCreate(id) is { } target)
                await target.ShowAsync();
        }

        public bool IsVisible(string id) =>
            _panels.TryGetValue(id, out var panel) && panel.IsVisible;

        private IPanel GetOrCreate(string id)
        {
            if (_panels.TryGetValue(id, out var existing)) return existing;

            var prefab = _settings.GetPrefab(id);
            if (prefab == null) return null;

            var instance = Object.Instantiate(prefab);
            GameObjectInjector.InjectRecursive(instance.gameObject, _container);
            Object.DontDestroyOnLoad(instance.gameObject);

            _panels[id] = instance;
            return instance;
        }
    }
}
