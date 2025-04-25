using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class PlaysSoundOnCollision : MonoBehaviour
{
    [SerializeField] public AudioClip audioClip;
    [SerializeField] public float clipVolume = 1f;
    [SerializeField] public float clipCooldown = 1f;

    private float cooldown = 0f;
    private bool soundPlayed = false;

    void Update()
    {
        if (!soundPlayed) return;
        
        cooldown += Time.deltaTime;

        if (!(cooldown >= clipCooldown)) return;
        
        soundPlayed = false;
        cooldown = 0f;
    }
    public void OnCollisionEnter(Collision other)
    {
        if (!other.collider.CompareTag("Player") || soundPlayed) return;
        SoundFXManager.instance.PlaySoundFxClip(audioClip, transform, clipVolume);
        soundPlayed = true;
    }
    
}
