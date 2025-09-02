using System.Collections.Generic;
using UnityEngine;

namespace Interfaces.Managers
{
    public interface IResourcesManager
    {
        public T LoadEntity<T>(string path) where T : Object;
        public List<T> LoadEntities<T>(string path) where T : Object;
    }
}