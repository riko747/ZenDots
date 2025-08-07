using UnityEngine;

namespace Interfaces.Managers
{
    public interface IAudioManager
    {
        public void PlaySound(AudioSource audioSource, AudioClip audioClip);
    }
}