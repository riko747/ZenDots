using Interfaces;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneLoadManager : ISceneLoadManager
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
