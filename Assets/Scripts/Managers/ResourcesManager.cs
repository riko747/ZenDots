using Interfaces;
using UnityEngine;

namespace Managers
{
    public class ResourcesManager : IResourcesManager
    {
        public T LoadEntity<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }
    }
}
