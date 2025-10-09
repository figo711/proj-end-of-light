using UnityEngine;

namespace Game
{
    public class PlayerSounds : MonoBehaviour
    {
        public static PlayerSounds Instance;

        [Header("Audio Sources")]
        public AudioSource audioSource;

        [Header("Clips")]
        public AudioClip walkClip;
        public AudioClip runClip;
        public AudioClip crouchClip;
        public AudioClip pickKeyClip;
        public AudioClip throwRockClip;

        private void Awake()
        {
            Instance = this;
        }

        void PlaySound(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void OnWalk()
        {
            PlaySound(walkClip);
        }

        public void OnRun()
        {
            PlaySound(runClip);
        }

        public void OnCrouch()
        {
            PlaySound(crouchClip);
        }

        public void OnPickKey()
        {
            PlaySound(pickKeyClip);
        }

        public void OnThrowRock()
        {
            PlaySound(throwRockClip);
        }
    }
}
