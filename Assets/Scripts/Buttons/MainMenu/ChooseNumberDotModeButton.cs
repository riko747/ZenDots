using Buttons.DefaultMode;
using Interfaces.Managers;
using Managers;
using Other;
using Zenject;

namespace Buttons.MainMenu
{
    public class ChooseNumberDotModeButton : UIButton
    {
        [Inject] private ISceneLoadManager _sceneLoadManager;
        [Inject] private IPlayerPrefsManager _playerPrefsManager;
        protected override void HandleButton()
        {
            _playerPrefsManager.SaveKey(Constants.CurrentDotMode, Constants.NumberDotMode);
            _sceneLoadManager.LoadScene(Constants.GameSceneName);
        }
    }
}