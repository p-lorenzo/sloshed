using System;
using UnityEngine;

public class ThisManBehavior : MonoBehaviour
{
    [SerializeField] private float spawnTimer;
    
    private float deathTimer;
    private AudioSource audioSource;
    private MeshRenderer renderer;
    private bool jumpScared = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        renderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (jumpScared) return;
        if (!other.CompareTag("PlayerHead")) return;
        deathTimer += Time.deltaTime;
        if (deathTimer >= spawnTimer) JumpScare();
    }

    private void JumpScare()
    {
        audioSource.Play();
        renderer.enabled = true;
        jumpScared = true;
    }
}
