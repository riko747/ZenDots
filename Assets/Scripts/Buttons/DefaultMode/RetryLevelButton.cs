using Interfaces.Managers;
using Other;
using Zenject;

namespace Buttons.DefaultMode
{
    public class RetryLevelButton : UIButton
    {
        [Inject] private ISceneLoadManager sceneLoadManager;
        protected override void HandleButton() => sceneLoadManager.LoadScene(Constants.GameSceneName);
    }
}
