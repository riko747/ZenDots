using System;
using Entities.Dot;
using Other;
using Zenject;

namespace Interfaces
{
    internal interface IGameManager
    {
        public void Initialize();
        public void TrySelect(Dot dot);
        public IInstantiator Instantiator { get; }
        public Constants.GameMode GetGameMode();
    }
}
