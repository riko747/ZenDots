using Buttons.DefaultMode;
using Managers;
using Other;
using Zenject;

namespace Buttons.MainMenu
{
    public class ChooseColorDotModeButton : UIButton
    {
        [Inject] private SceneLoadManager _sceneLoadManager;
        [Inject] private PlayerPrefsManager _playerPrefsManager;
        
        protected override void HandleButton()
        {
            _playerPrefsManager.SaveKey(Constants.CurrentDotMode, Constants.ColorDotMode);
            _sceneLoadManager.LoadScene(Constants.GameSceneName);
        }
    }
}