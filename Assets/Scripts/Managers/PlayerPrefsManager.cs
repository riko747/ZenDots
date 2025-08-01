using System;
using Interfaces;
using UnityEngine;

namespace Managers
{
    public class PlayerPrefsManager : IPlayerPrefsManager
    {
        public void SaveKey<T>(string key, T value)
        {
            switch (value)
            {
                case int:
                    PlayerPrefs.SetInt(key, Convert.ToInt32(value));
                    break;
                case float:
                    PlayerPrefs.SetFloat(key, Convert.ToSingle(value));
                    break;
                case string:
                    PlayerPrefs.SetString(key, value.ToString());
                    break;
            }

            PlayerPrefs.Save();
        }

        public T LoadKey<T>(string key)
        {
            try
            {
                if (!PlayerPrefs.HasKey(key))
                    return default;

                var type = typeof(T);

                if (type == typeof(int))
                    return (T)(object)PlayerPrefs.GetInt(key);
                if (type == typeof(float))
                    return (T)(object)PlayerPrefs.GetFloat(key);
                if (type == typeof(string))
                    return (T)(object)PlayerPrefs.GetString(key);

                throw new NotSupportedException($"Type {type} is not supported by PlayerPrefs");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return default;
            }
        }
    }
}