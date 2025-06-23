using Level;

namespace Interfaces
{
    public interface ILevelManager
    {
        public void Initialize();
        public LevelData GetCurrentLevel();
    }
}