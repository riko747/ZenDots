using Buttons.DefaultMode;
using Interfaces.Managers;
using Managers;
using Other;
using Zenject;

namespace Buttons.MainMenu
{
    public class ChooseZenModeButton : UIButton
    {
        [Inject] private IPlayerPrefsManager _playerPrefsManager;
        [Inject] private MenuUIManager _menuUIManager;

        protected override void HandleButton()
        {
            _playerPrefsManager.SaveKey(Constants.CurrentGameMode, Constants.ZenGameMode);
            _menuUIManager.ProceedToDotModeSelection();
        }
    }
}
