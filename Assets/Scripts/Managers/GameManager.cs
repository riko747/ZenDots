using Interfaces;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class GameManager : IGameManager
    {
        [Inject] public void InitializeInstantiator(IInstantiator instantiator) => Instantiator = instantiator;
        [Inject] private DotSpawner _dotSpawner;
        [Inject] private UIManager _uiManager;
        
        private int _expectedNumber;
        
        public IInstantiator Instantiator { get; private set; }

        public void Initialize()
        {
            _expectedNumber = 1;
        }

        public void TrySelect(int tappedValue)
        {
            if (tappedValue == _expectedNumber)
            {
                _expectedNumber++;

                if (tappedValue == _dotSpawner.GetMaxDots())
                {
                    Debug.Log("Level is completed");
                    _uiManager.ShowUIButton();
                }
            }
            else
            {
                Debug.Log("Wrong");
                _uiManager.ShowUIButton();
            }
        }
    }
}
