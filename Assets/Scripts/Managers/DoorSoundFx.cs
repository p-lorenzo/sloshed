using System;
using UnityEngine;
using UnityEngine.Audio;

public class DoorSoundFx : MonoBehaviour
{
    [SerializeField] public AudioClip soundFxClip;
    [SerializeField] public float clipVolume = 1f;
    [SerializeField] public float clipCooldown = 1f;

    private float cooldown = 0f;
    private bool doorOpen = false;

    void Update()
    {
        if (!doorOpen) return;
        
        cooldown += Time.deltaTime;

        if (!(cooldown >= clipCooldown)) return;
        
        doorOpen = false;
        cooldown = 0f;
    }
    public void OnCollisionEnter(Collision other)
    {
        Debug.Log($"{other.collider.name} - {other.collider.tag}");
        if (!other.collider.CompareTag("Player") || doorOpen) return;
        SoundFXManager.instance.PlaySoundFxClip(soundFxClip, transform, clipVolume);
        doorOpen = true;
    }
    
}
