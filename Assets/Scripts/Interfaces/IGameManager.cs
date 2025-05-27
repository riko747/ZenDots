using Zenject;

namespace Interfaces
{
    internal interface IGameManager
    {
        public void Initialize();
        public void TrySelect(int tappedValue);
        public IInstantiator Instantiator { get; }
    }
}
