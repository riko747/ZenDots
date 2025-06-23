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
                if (PlayerPrefs.HasKey(key))
                {
                    return (T)Convert.ChangeType(PlayerPrefs.GetInt(key), typeof(T));
                }
            }
            catch (Exception)
            {
                Debug.LogException(new Exception("There is no any keys with this name in PlayerPrefs"));
            }
            return default;
        }
    }
}