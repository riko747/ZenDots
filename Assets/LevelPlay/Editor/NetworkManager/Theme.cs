using System;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.LevelPlay.Editor
{
    struct Colors
    {
        internal Color BackgroundColor;
        internal Color BorderColor;
        internal Color RowColor1;
        internal Color RowColor2;
    }

    static class Theme
    {
        static Colors Dark = new Colors
        {
            BackgroundColor = new Color(56 / 255f, 56 / 255f, 56 / 255f, 1),
            BorderColor = new Color(35 / 255f, 35 / 255f, 35 / 255f, 1),
            RowColor1 = new Color(63 / 255f, 63 / 255f, 63 / 255f, 1),
            RowColor2 = new Color(56 / 255f, 56 / 255f, 56 / 255f, 1),
        };

        static Colors Light = new Colors
        {
            BackgroundColor = new Color(200 / 255f, 200 / 255f, 200 / 255f, 1),
            BorderColor = new Color(35 / 255f, 35 / 255f, 35 / 255f, 1),
            RowColor1 = new Color(240 / 255f, 240 / 255f, 240 / 255f, 1),
            RowColor2 = new Color(200 / 255f, 200 / 255f, 200 / 255f, 1),
        };

        internal static Colors GetColors()
        {
            return EditorGUIUtility.isProSkin ? Dark : Light;
        }
    }
}
