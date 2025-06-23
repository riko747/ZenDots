namespace Interfaces
{
    public interface IPlayerPrefsManager
    {
        public void SaveKey<T>(string key, T value);
        public T LoadKey<T>(string key);
    }
}