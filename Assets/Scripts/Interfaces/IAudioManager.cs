using UnityEngine;

namespace Interfaces
{
    public interface IAudioManager
    {
        public void PlaySound(AudioSource audioSource, AudioClip audioClip);
    }
}