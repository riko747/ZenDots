using Interfaces;
using UnityEngine;
using Zenject;

namespace Buttons
{
    public class DotButton : UIButton
    {
        [Inject] private IGameManager _gameManager;

        [SerializeField] private Dot dot;
        
        protected override void HandleButton()
        {
            var number = dot.DotNumber;
            _gameManager.TrySelect(number);
            Destroy(gameObject);
        }
    }
}
