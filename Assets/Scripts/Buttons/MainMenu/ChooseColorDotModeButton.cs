using Buttons.DefaultMode;
using Interfaces.Managers;
using Managers;
using Other;
using Zenject;

namespace Buttons.MainMenu
{
    public class ChooseColorDotModeButton : UIButton
    {
        [Inject] private ISceneLoadManager _sceneLoadManager;
        [Inject] private IPlayerPrefsManager _playerPrefsManager;
        
        protected override void HandleButton()
        {
            _playerPrefsManager.SaveKey(Constants.CurrentDotMode, Constants.ColorDotMode);
            _sceneLoadManager.LoadScene(Constants.GameSceneName);
        }
    }
}