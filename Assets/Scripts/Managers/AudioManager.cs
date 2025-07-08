using UnityEngine;

namespace Managers
{
    public class AudioManager
    {
        public void PlaySound(AudioSource audioSource, AudioClip audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}