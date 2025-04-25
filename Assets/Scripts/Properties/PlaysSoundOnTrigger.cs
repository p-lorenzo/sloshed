using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class PlaysSoundOnTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (audioSource.isPlaying) return;
        audioSource.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!audioSource.isPlaying) return;
        audioSource.Stop();
    }
}
