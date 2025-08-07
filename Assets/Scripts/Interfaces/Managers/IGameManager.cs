using System;
using Interfaces.Strategies;
using Zenject;

namespace Interfaces.Managers
{
    internal interface IGameManager
    {
        public void Initialize();
        public IInstantiator Instantiator { get; }
        public IValidateStrategy  ValidateStrategy { get; }
        public string GetGameMode();
        public Action OnRightDotClicked { get; set; }
        public Action OnLevelCompleted { get; set; }
    }
}
