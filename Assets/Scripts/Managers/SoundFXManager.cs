using UnityEngine;

namespace Proyecto3.Managers.SoundManager
{
    // https://www.youtube.com/watch?v=DU7cgVsU2rM
    public class SoundFXManager : MonoBehaviour
    {
        // make singleton (only one in the scene)
        public static SoundFXManager instance;
        [SerializeField] private AudioSource soundFXObject;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public void PlaySoundFXClip(AudioClip clip, Transform transform, float volume)
        {
            // spanwn in gameObject
            AudioSource audioSource = Instantiate(soundFXObject, transform.position, Quaternion.identity);

            // assign the audioclip
            audioSource.clip = clip;

            // assign volume
            audioSource.volume = volume;

            // play the sound
            audioSource.Play();

            // get length of clip
            float clipLength = audioSource.clip.length;

            // destroy after clip length
            Destroy(audioSource.gameObject, clipLength);
        }

        public void PlayRandomSoundFXClip(AudioClip[] clips, Transform transform, float volume)
        {
            // pick a random clip from the array
            AudioClip clip = clips[Random.Range(0, clips.Length)];

            // spanwn in gameObject
            AudioSource audioSource = Instantiate(soundFXObject, transform.position, Quaternion.identity);

            // assign the audioclip
            audioSource.clip = clip;

            // assign volume
            audioSource.volume = volume;

            // play the sound
            audioSource.Play();

            // get length of clip
            float clipLength = audioSource.clip.length;

            // destroy after clip length
            Destroy(audioSource.gameObject, clipLength);
        }


    }
}