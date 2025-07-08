namespace Other
{
    public static class Constants
    {
        //Enums
        public enum GameMode {Default, Zen}
        
        // Core
        public const float MinDotSize = 100;
        public const float MaxDotSize = 250;
        
        //Paths
        public const string DotPrefabPath = "Entities/Dot";
        public const string LevelsPath = "Levels";
        
        //Scenes
        public const string MainMenuSceneName = "MainMenu";
        public const string DefaultModeSceneName = "DefaultMode";
        public const string ZenModeSceneName = "ZenMode";
        
        //PlayerPrefs
        public const string CurrentLevel = "CurrentLevel";
        public const string CurrentGameMode = "CurrentGameMode";
    }
}
