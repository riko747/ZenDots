using UnityEngine.SceneManagement;

namespace Buttons.DefaultMode
{
    public class RetryLevelButton : UIButton
    {
        protected override void HandleButton() => SceneManager.LoadScene(0);
    }
}
