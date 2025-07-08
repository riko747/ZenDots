using Buttons.DefaultMode;
using Managers;
using Other;
using Zenject;

namespace Buttons.MainMenu
{
    public class StartDefaultModeButton : UIButton
    {
        [Inject] private SceneLoadManager _sceneLoadManager;
        [Inject]  private PlayerPrefsManager _playerPrefsManager;
        
        protected override void HandleButton()
        {
            _playerPrefsManager.SaveKey(Constants.CurrentGameMode, Constants.GameMode.Default);
            _sceneLoadManager.LoadScene(Constants.DefaultModeSceneName);
        }
    }
}
