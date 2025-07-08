using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/SoundLibrary")]
    public class SoundLibrary : ScriptableObject
    {
        public AudioClip popSound;
    }
}
