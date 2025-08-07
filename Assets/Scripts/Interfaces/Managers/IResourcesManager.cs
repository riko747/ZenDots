namespace Interfaces.Managers
{
    internal interface IResourcesManager
    {
        public T LoadEntity<T>(string path) where T : UnityEngine.Object;
    }
}