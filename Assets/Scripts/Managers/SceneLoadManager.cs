using Interfaces.Managers;
using Other;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneLoadManager : ISceneLoadManager
    {
        public void AttachToGameManager(GameManager gameManager)
        {
            gameManager.OnLevelCompleted += () =>
                LoadScene(Constants.GameSceneName);
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
