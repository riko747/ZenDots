using Level;

namespace Interfaces.Managers
{
    public interface ILevelManager
    {
        public void Initialize();
        public LevelData GetCurrentLevel();
    }
}