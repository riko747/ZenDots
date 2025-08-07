using Interfaces;
using Interfaces.Managers;
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
            return GetLevelFromDataBase(PlayerPrefs.HasKey(Constants.CurrentLevel) ? PlayerPrefs.GetInt(Constants.CurrentLevel) : 0);
        }
        
        public void SaveCurrentLevel()
        {
            _playerPrefsManager.SaveKey(Constants.CurrentLevel, PlayerPrefs.GetInt(Constants.CurrentLevel) + 1);
        }
    }
}