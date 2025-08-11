using System;
using Core;
using Core.Spawners;
using Core.Validators;
using Interfaces.Managers;
using Interfaces.Strategies;
using Other;
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
        [Inject] private SceneLoadManager _sceneLoadManager;
        
        private string _gameMode;
        
        public IInstantiator Instantiator { get; private set; }
        public IValidateStrategy  ValidateStrategy { get; private set; }
        public Action OnRightDotClicked { get; set; }
        public Action OnLevelCompleted { get; set; }

        public void Initialize()
        {
            _sceneLoadManager.AttachToGameManager(this);
            InitializeGameMode();
            InitializeValidator();
        }

        public string GetGameMode() => _gameMode;
        
        private void InitializeGameMode()
        {
            var currentGameMode = _playerPrefsManager.LoadKey<string>(Constants.CurrentGameMode);
            
            if (currentGameMode == Constants.DefaultGameMode)
            {
                _gameMode = Constants.DefaultGameMode;
            }

            if (currentGameMode == Constants.ZenGameMode)
            {
                _gameMode = Constants.ZenGameMode;
            }
        }

        private void InitializeValidator()
        {
            var currentDotMode = _playerPrefsManager.LoadKey<string>(Constants.CurrentDotMode);

            if (currentDotMode == Constants.NumberDotMode)
            {
                ValidateStrategy = Instantiator.Instantiate<NumberValidator>(new object[] { _gameMode });
            }

            if (currentDotMode == Constants.ColorDotMode)
            {
                ValidateStrategy = Instantiator.Instantiate<ColorValidator>(new object[] { _gameMode });
            }
        }
    }
}
