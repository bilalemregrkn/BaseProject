using Plugins.EventBus;
using UnityEngine;

namespace Plugins.SaveManagement
{
    public sealed class SaveManager : ISaveManager
    {
        private readonly IEventBus _eventBus;

        public SaveManager(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Save<T>(string key, T data)
        {
            if (string.IsNullOrEmpty(key)) return;

            var json = JsonUtility.ToJson(new Wrapper<T>(data));
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();

            _eventBus.Publish(new DataSaved(key));
        }

        public T Load<T>(string key, T defaultValue = default)
        {
            if (string.IsNullOrEmpty(key) || !PlayerPrefs.HasKey(key))
                return defaultValue;

            var json = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<Wrapper<T>>(json).Value;
        }

        public bool HasKey(string key) => !string.IsNullOrEmpty(key) && PlayerPrefs.HasKey(key);

        public void Delete(string key)
        {
            if (string.IsNullOrEmpty(key) || !PlayerPrefs.HasKey(key)) return;

            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();

            _eventBus.Publish(new DataDeleted(key));
        }

        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            _eventBus.Publish(new AllDataDeleted());
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T Value;

            public Wrapper(T value) => Value = value;
        }
    }
}
