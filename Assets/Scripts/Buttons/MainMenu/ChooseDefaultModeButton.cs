using Buttons.DefaultMode;
using Managers;
using Other;
using Zenject;

namespace Buttons.MainMenu
{
    public class ChooseDefaultModeButton : UIButton
    {
        [Inject] private PlayerPrefsManager _playerPrefsManager;
        [Inject] private UIManager _uiManager;

        protected override void HandleButton()
        {
            _playerPrefsManager.SaveKey(Constants.CurrentGameMode, Constants.DefaultGameMode);
            _uiManager.ProceedToDotModeSelection();
        }
    }
}
