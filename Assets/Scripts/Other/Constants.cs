namespace Other
{
    public static class Constants
    {
        // Core
        public const float MinDotSize = 100;
        public const float MaxDotSize = 250;
        public const int MaxChecks = 10000;
        public const int ZenModeDotsStartCount = 9;
        
        //Paths
        public const string DotPrefabPath = "Entities/Dot";
        public const string LevelsPath = "Levels";
        
        //Scenes
        public const string MainMenuSceneName = "MainMenu";
        public const string GameSceneName = "Game";
        
        //GameModes
        public const string DefaultGameMode = "DefaultGameMode";
        public const string ZenGameMode = "ZenGameMode";
        
        //DotModes
        public const string NumberDotMode = "NumberDotMode";
        public const string ColorDotMode = "ColorDotMode";
        
        //PlayerPrefs
        public const string CurrentLevel = "CurrentLevel";
        public const string CurrentGameMode = "CurrentGameMode";
        public const string CurrentDotMode = "CurrentDotMode";
        
        //Animation values
        public const float IdleAnimationScaleMin = 0.8f;
        public const float IdleAnimationScaleMax = 1.2f;
        
        //Spawn constants
        public const float CollisionPaddingPx   = 2f;
        public const float PopScalePeak         = 1.2f;
        public const int   SeparationIterations = 250;
        public const float SeparationEpsilon    = 1.2f;
        public const float RectWidthEpsilon     = 1e-4f;

        public const float MinDistanceEpsilon   = 1e-6f;
        public static readonly UnityEngine.Vector2 FallbackNormal = new(0.70710678f, 0.70710678f);

    }
}
