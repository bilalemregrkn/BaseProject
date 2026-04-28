using System;

namespace Backend.Systems.EventBus
{
    public interface IEventBus
    {
        void Subscribe<TEvent>(Action<TEvent> handler, string scope = EventBus.GlobalScope);
        void Unsubscribe<TEvent>(Action<TEvent> handler, string scope = EventBus.GlobalScope);
        void Publish<TEvent>(TEvent evt, string scope = EventBus.GlobalScope);
        void ClearScope(string scope);
    }
}