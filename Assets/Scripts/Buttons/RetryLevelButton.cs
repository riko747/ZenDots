using UnityEngine.SceneManagement;

namespace Buttons
{
    public class RetryLevelButton : UIButton
    {
        protected override void HandleButton()
        {
            SceneManager.LoadScene(0);
        }
    }
}
