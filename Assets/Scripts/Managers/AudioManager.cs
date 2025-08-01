using Interfaces;
using UnityEngine;

namespace Managers
{
    public class AudioManager : IAudioManager
    {
        public void PlaySound(AudioSource audioSource, AudioClip audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}