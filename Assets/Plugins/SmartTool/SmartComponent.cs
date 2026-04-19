using System.Collections.Generic;
using UnityEngine;

namespace Tools.SmartComponent
{
    public enum Source
    {
        Self,
        Parent,
        Children
    }

    public abstract class SmartComponent : MonoBehaviour
    {
        private static class ComponentCache<T>
        {
            internal static readonly Dictionary<(int id, Source source, bool includeInactive), T> Single
                = new Dictionary<(int, Source, bool), T>();

            internal static readonly Dictionary<(int id, Source source, bool includeInactive), List<T>> Multi
                = new Dictionary<(int, Source, bool), List<T>>();
        }

        // Registered clearers per type, populated on first Get/GetAll call for that T
        private static readonly List<System.Action<int>> _typeClearers = new List<System.Action<int>>();
        private static readonly HashSet<System.Type> _registeredTypes = new HashSet<System.Type>();

        private static void EnsureRegistered<T>()
        {
            if (!_registeredTypes.Add(typeof(T)))
                return;

            _typeClearers.Add(id =>
            {
                ComponentCache<T>.Single.Remove((id, Source.Self,     true));
                ComponentCache<T>.Single.Remove((id, Source.Self,     false));
                ComponentCache<T>.Single.Remove((id, Source.Parent,   true));
                ComponentCache<T>.Single.Remove((id, Source.Parent,   false));
                ComponentCache<T>.Single.Remove((id, Source.Children, true));
                ComponentCache<T>.Single.Remove((id, Source.Children, false));
                ComponentCache<T>.Multi.Remove((id,  Source.Self,     true));
                ComponentCache<T>.Multi.Remove((id,  Source.Self,     false));
                ComponentCache<T>.Multi.Remove((id,  Source.Parent,   true));
                ComponentCache<T>.Multi.Remove((id,  Source.Parent,   false));
                ComponentCache<T>.Multi.Remove((id,  Source.Children, true));
                ComponentCache<T>.Multi.Remove((id,  Source.Children, false));
            });
        }

        protected T Get<T>(Source source = Source.Children, bool includeInactive = true)
        {
            EnsureRegistered<T>();

            var key = (GetInstanceID(), source, includeInactive);

            if (ComponentCache<T>.Single.TryGetValue(key, out T cached))
                return cached;

            T result = source switch
            {
                Source.Self     => GetComponent<T>(),
                Source.Parent   => GetComponentInParent<T>(includeInactive),
                Source.Children => GetComponentInChildren<T>(includeInactive),
                _               => default
            };

            ComponentCache<T>.Single[key] = result;
            return result;
        }

        protected List<T> GetAll<T>(Source source = Source.Children, bool includeInactive = true)
        {
            EnsureRegistered<T>();

            var key = (GetInstanceID(), source, includeInactive);

            if (ComponentCache<T>.Multi.TryGetValue(key, out List<T> cached))
                return cached;

            T[] found = source switch
            {
                Source.Self     => GetComponents<T>(),
                Source.Parent   => GetComponentsInParent<T>(includeInactive),
                Source.Children => GetComponentsInChildren<T>(includeInactive),
                _               => System.Array.Empty<T>()
            };

            var list = new List<T>(found);
            ComponentCache<T>.Multi[key] = list;
            return list;
        }

        protected virtual void OnDestroy()
        {
            int id = GetInstanceID();
            foreach (var cleaner in _typeClearers)
                cleaner(id);
        }
    }
}
