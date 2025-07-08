using Buttons.DefaultMode;
using UnityEngine;

namespace Buttons.MainMenu
{
    public class ExitGameButton : UIButton
    {
        protected override void HandleButton() => Application.Quit();
    }
}