using System;
using UnityEngine;
using UnityEngine.Audio;

public class TeddySoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource teddySound;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (teddySound.isPlaying) return;
        teddySound.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!teddySound.isPlaying) return;
        teddySound.Stop();
    }
}
