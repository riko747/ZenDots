using Buttons.DefaultMode;
using Interfaces.Managers;
using Other;
using Zenject;

namespace Buttons.MainMenu
{
    public class ChooseZenModeButton : UIButton
    {
        [Inject] private IPlayerPrefsManager _playerPrefsManager;
        [Inject] private IUIManager _uiManager;

        protected override void HandleButton()
        {
            _playerPrefsManager.SaveKey(Constants.CurrentGameMode, Constants.ZenGameMode);
            _uiManager.ProceedToDotModeSelection();
        }
    }
}
