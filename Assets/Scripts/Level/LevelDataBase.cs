using System.Collections.Generic;
using System.Linq;
using Managers;
using Other;
using Zenject;

namespace Level
{
    public class LevelDataBase
    {
        [Inject] private ResourcesManager _resourcesManager;
        
        private List<LevelData> _levels;

        public void Initialize()
        {
            _levels = _resourcesManager.LoadEntities<LevelData>(Constants.LevelsPath).OrderBy(level => level.levelNumber).ToList();
        }

        public LevelData GetLevel(int levelNumber)
        {
            return levelNumber >= _levels.Count ? 
                _levels[0] : 
                _levels.FirstOrDefault(level => level.levelNumber == levelNumber);
        }
    }
}
