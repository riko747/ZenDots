using System;
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
        
        private string _gameMode;
        
        private int _expectedNumber;
        public IInstantiator Instantiator { get; private set; }
        
        public Action OnRightDotClicked { get; set; }
        
        public void Initialize()
        {
            _expectedNumber = 1;
            InitializeGameMode();
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

        public void TrySelect(Dot dot)
        {
                if (dot.DotNumber == _expectedNumber)
                {
                    _expectedNumber++;
                    dot.GetDotButton().PlayCorrectSequence();
                    if (_gameMode == Constants.DefaultGameMode)
                    {
                        if (dot.IsLast)
                        {
                            _levelManager.SaveCurrentLevel();
                            SceneManager.LoadScene(Constants.GameSceneName);
                        }
                    }
                    else
                    {
                        OnRightDotClicked?.Invoke();
                    }
                }
                else
                {
                    dot.GetDotButton().PlayWrongSequence();
                }
        }
    }
}
