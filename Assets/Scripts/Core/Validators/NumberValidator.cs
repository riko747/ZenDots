using Entities.Dot;
using Interfaces.Strategies;
using Managers;
using Other;
using Zenject;

namespace Core.Validators
{
    public class NumberValidator : IValidateStrategy
    {
        [Inject] private LevelManager _levelManager;
        [Inject] private GameManager _gameManager;
        
        private readonly string _gameMode;   
        private int _expectedNumber = 1;

        public NumberValidator(string gameMode)
        {
            _gameMode = gameMode;
        }
        
        public void Validate(Dot dot)
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
                        _gameManager.OnLevelCompleted?.Invoke();
                    }
                }
                else
                {
                    _gameManager.OnRightDotClicked?.Invoke();
                }
            }
            else
            {
                dot.GetDotButton().PlayWrongSequence();
            }
        }
    }
}