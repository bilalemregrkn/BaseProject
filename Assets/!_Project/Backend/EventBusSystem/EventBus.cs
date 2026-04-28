using System;
using System.Collections.Generic;

namespace Plugins.EventBus
{
    public sealed class EventBus : IEventBus
    {
        public const string GlobalScope = "global";

        private readonly Dictionary<string, Dictionary<Type, List<Delegate>>> _scopes = new();

        public void Subscribe<TEvent>(Action<TEvent> handler, string scope = GlobalScope)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            scope ??= GlobalScope;

            if (!_scopes.TryGetValue(scope, out var typeMap))
            {
                typeMap = new Dictionary<Type, List<Delegate>>(8);
                _scopes.Add(scope, typeMap);
            }

            var type = typeof(TEvent);

            if (!typeMap.TryGetValue(type, out var list))
            {
                list = new List<Delegate>(4);
                typeMap.Add(type, list);
            }

            if (!list.Contains(handler))
                list.Add(handler);
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler, string scope = GlobalScope)
        {
            if (handler == null) return;
            scope ??= GlobalScope;

            if (!_scopes.TryGetValue(scope, out var typeMap)) return;

            var type = typeof(TEvent);
            if (!typeMap.TryGetValue(type, out var list)) return;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Equals(handler))
                    list.RemoveAt(i);
            }

            if (list.Count == 0)
                typeMap.Remove(type);

            if (typeMap.Count == 0)
                _scopes.Remove(scope);
        }

        public void Publish<TEvent>(TEvent evt, string scope = GlobalScope)
        {
            scope ??= GlobalScope;

            if (!_scopes.TryGetValue(scope, out var typeMap)) return;

            var type = typeof(TEvent);
            if (!typeMap.TryGetValue(type, out var list)) return;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] is Action<TEvent> action)
                    action(evt);
            }
        }

        public void ClearScope(string scope)
        {
            if (scope == null) return;
            _scopes.Remove(scope);
        }
    }
}