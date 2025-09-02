using Buttons.DefaultMode;
using Interfaces.Managers;
using Other;
using Zenject;

namespace Buttons.MainMenu
{
    public class ChooseDefaultModeButton : UIButton
    {
        [Inject] private IPlayerPrefsManager _playerPrefsManager;
        [Inject] private IUIManager _uiManager;

        protected override void HandleButton()
        {
            _playerPrefsManager.SaveKey(Constants.CurrentGameMode, Constants.DefaultGameMode);
            _uiManager.ProceedToDotModeSelection();
        }
    }
}
