using UnityEngine;
namespace Yudiz.DirtBikeVR.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        public AudioSource audioSource;
        public AudioSource oneShotSounds;

        public Sound[] sound;

        private void Awake()
        {
            instance = this;
        }

        public void PlaySound(AudioTrack name, bool loop)
        {
            //if (audioSource.isPlaying) { audioSource.Stop(); }

            foreach (Sound sounds in sound)
            {
                if (sounds.name == name && loop)
                {
                    //audioSource.PlayOneShot(sounds.clip);
                    audioSource.clip = sounds.clip;
                    audioSource.Play();
                    audioSource.loop = loop;
                }

                if (sounds.name == name && !loop)
                {
                    Debug.LogWarning("OneShot");
                    oneShotSounds.PlayOneShot(sounds.clip);
                }
            }
        }

        public void StopSound()
        {
            audioSource.Stop();
        }
    }

    [System.Serializable]
    public class Sound
    {
        public AudioTrack name;
        public AudioClip clip;
    }

    public enum AudioTrack
    {
        None,
        BikeStart,
        BikeEngine,
        BikeRunning,
    }
}
