using System;
using System.Collections;
using UnityEngine;

public class HolyWaterManager : MonoBehaviour
{
    [SerializeField] private float startingSize = 1f;
    [SerializeField] private float finalSize = 5f;
    [SerializeField] private float shieldDuration = 2f;
    
    private SphereCollider sphereCollider;
    private ParticleSystem holyWaterParticle;
    private AudioSource audioSource;
    private bool isEnabled;
    private float elapsedTime;
    
    void Start()
    {
        if (sphereCollider == null)
            sphereCollider = gameObject.GetComponent<SphereCollider>();
        if (holyWaterParticle == null)
            holyWaterParticle = gameObject.GetComponentInChildren<ParticleSystem>();
        if (audioSource == null)
            audioSource = gameObject.GetComponent<AudioSource>();
        ResetGameObject();
    }

    private void ResetGameObject()
    {
        holyWaterParticle.Stop();
        sphereCollider.radius = 0f;
        elapsedTime = 0f;
        isEnabled = false;
    }

    void Update()
    {
        if (!isEnabled) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / .5f);
        sphereCollider.radius = Mathf.Lerp(startingSize, finalSize, t);

        if (elapsedTime >= shieldDuration) ResetGameObject();
    }

    public void OnItemUsed()
    {
        holyWaterParticle.Play();
        audioSource.Play();
        sphereCollider.radius = startingSize;
        isEnabled = true;
    }
}