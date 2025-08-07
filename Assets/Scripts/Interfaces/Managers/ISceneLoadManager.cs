using Managers;

namespace Interfaces.Managers
{
    public interface ISceneLoadManager
    {
        public void LoadScene(string sceneName);
        public void AttachToGameManager(GameManager gameManager);
    }
}