using Interfaces;
using Level;
using Other;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class LevelManager : ILevelManager
    {
        [Inject] PlayerPrefsManager _playerPrefsManager;
        [Inject] GameManager _gameManager;
        
        private LevelDataBase _levelDataBase;

        private LevelData GetLevelFromDataBase(int levelId)
        {
            return _levelDataBase.GetLevel(levelId);
        }
        
        public void Initialize()
        {
            _levelDataBase = _gameManager.Instantiator.Instantiate<LevelDataBase>();
            _levelDataBase.Initialize();
        }

        public LevelData GetCurrentLevel()
        {
            return GetLevelFromDataBase(PlayerPrefs.HasKey(Constants.CURRENT_LEVEL) ? PlayerPrefs.GetInt(Constants.CURRENT_LEVEL) : 0);
        }
        
        public void SaveCurrentLevel()
        {
            _playerPrefsManager.SaveKey(Constants.CURRENT_LEVEL, PlayerPrefs.GetInt(Constants.CURRENT_LEVEL) + 1);
        }
    }
}