using System;
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
        private Dictionary<(Type type, string scope, bool isList), object> _cache;

        protected T Get<T>(Source source = Source.Children, bool includeInactive = true)
        {
            _cache ??= new Dictionary<(Type, string, bool), object>();

            var scope = GetScope(source, includeInactive);
            var key = (typeof(T), scope, false);

            if (_cache.TryGetValue(key, out var cached) && cached is T cachedResult)
                return cachedResult;

            T result = source switch
            {
                Source.Self => GetComponent<T>(),
                Source.Parent => GetComponentInParent<T>(includeInactive),
                Source.Children => GetComponentInChildren<T>(includeInactive),
                _ => default
            };

            _cache[key] = result;
            return result;
        }

        protected List<T> GetAll<T>(Source source = Source.Children, bool includeInactive = true)
        {
            _cache ??= new Dictionary<(Type, string, bool), object>();

            var scope = GetScope(source, includeInactive);
            var key = (typeof(T), scope, true);

            if (_cache.TryGetValue(key, out var cached) && cached is List<T> cachedList)
                return cachedList;

            T[] found = source switch
            {
                Source.Self => GetComponents<T>(),
                Source.Parent => GetComponentsInParent<T>(includeInactive),
                Source.Children => GetComponentsInChildren<T>(includeInactive),
                _ => Array.Empty<T>()
            };

            var list = new List<T>(found);
            _cache[key] = list;
            return list;
        }

        private static string GetScope(Source source, bool includeInactive)
        {
            return source switch
            {
                Source.Self => "self",
                Source.Parent => includeInactive ? "parent_inactive" : "parent",
                Source.Children => includeInactive ? "children_inactive" : "children",
                _ => "self"
            };
        }

        
    }
}