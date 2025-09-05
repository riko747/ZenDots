using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/SoundLibrary")]
    public sealed class SoundLibrary : ScriptableObject
    {
        [SerializeField] private AudioClip popSound;
        public AudioClip PopSound => popSound;
    }
}
