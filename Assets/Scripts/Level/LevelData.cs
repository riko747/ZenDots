using UnityEngine;

namespace Level
{
    [CreateAssetMenu (fileName="LevelData", menuName = "Level/LevelData")]
    public class LevelData : ScriptableObject
    {
        public int levelNumber;
        public int dotCount;
        public float timeLimit;
    }
}
