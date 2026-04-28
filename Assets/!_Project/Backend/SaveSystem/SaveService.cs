using Backend.Systems.EventBus;
using UnityEngine;

namespace Backend.Systems.Save
{
    public sealed class SaveService : ISaveService
    {
        private readonly IEventBus _eventBus;

        public SaveService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Save<T>(string key, T data)
        {
            if (string.IsNullOrEmpty(key)) return;

            switch (data)
            {
                case int i:    PlayerPrefs.SetInt(key, i);    break;
                case float f:  PlayerPrefs.SetFloat(key, f);  break;
                case string s: PlayerPrefs.SetString(key, s); break;
                default:
                    PlayerPrefs.SetString(key, JsonUtility.ToJson(data));
                    break;
            }

            PlayerPrefs.Save();
            _eventBus.Publish(new DataSaved(key));
        }

        public T Load<T>(string key, T defaultValue = default)
        {
            if (string.IsNullOrEmpty(key) || !PlayerPrefs.HasKey(key))
                return defaultValue;

            if (typeof(T) == typeof(int))    return (T)(object)PlayerPrefs.GetInt(key);
            if (typeof(T) == typeof(float))  return (T)(object)PlayerPrefs.GetFloat(key);
            if (typeof(T) == typeof(string)) return (T)(object)PlayerPrefs.GetString(key);

            return JsonUtility.FromJson<T>(PlayerPrefs.GetString(key));
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
    }
}
