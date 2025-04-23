using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void PlaySoundFxClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.spatialize = true;
        audioSource.maxDistance = 5f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        
        audioSource.Play();
        
        float clipLength = audioSource.clip.length;
        
        Destroy(audioSource.gameObject, clipLength);
    }
    
}
