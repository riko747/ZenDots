using Core;
using Entities.Dot;
using Interfaces;
using Other;
using UnityEngine.SceneManagement;
using Zenject;

namespace Managers
{
    public class GameManager : IGameManager
    {
        [Inject] public void InitializeInstantiator(IInstantiator instantiator) => Instantiator = instantiator;
        
        [Inject] private DotSpawner _dotSpawner;
        [Inject] private UIManager _uiManager;
        [Inject] private LevelManager _levelManager;
        [Inject] private PlayerPrefsManager _playerPrefsManager;
        
        private Constants.GameMode _gameMode;
        
        private int _expectedNumber;
        public IInstantiator Instantiator { get; private set; }
        
        public void Initialize()
        {
            _expectedNumber = 1;
            InitializeGameMode();
        }

        public Constants.GameMode GetGameMode() => _gameMode;
        
        private void InitializeGameMode()
        {
            if (_playerPrefsManager.LoadKey<string>(Constants.CurrentGameMode) == nameof(Constants.GameMode.Default))
            {
                _gameMode = Constants.GameMode.Default;
            }

            if (_playerPrefsManager.LoadKey<string>(Constants.CurrentGameMode) == nameof(Constants.GameMode.Zen))
            {
                _gameMode = Constants.GameMode.Zen;
            }
        }

        public void TrySelect(Dot dot)
        {
            if (dot.DotNumber == _expectedNumber)
            {
                _expectedNumber++;
                dot.GetDotButton().PlayCorrectSequence();
                if (dot.IsLast)
                {
                    _levelManager.SaveCurrentLevel();
                    SceneManager.LoadScene(Constants.DefaultModeSceneName);
                }
            }
            else
            {
                dot.GetDotButton().PlayWrongSequence();
            }
        }
    }
}
