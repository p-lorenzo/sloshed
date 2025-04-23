using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public abstract class Powerup : MonoBehaviour, Pickupable
{
    [Header("Rotation and bobbing")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 90f, 0f);
    [SerializeField] private float bobbingAmplitude = 0.25f;
    [SerializeField] private float bobbingFrequency = 1f;
    [SerializeField] private GameObject visualObject;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private float volume = 1f;
    
    [Header("Pickup popup")]
    [SerializeField] private string pickupName;
    [SerializeField] private string pickupDescription;
    
    private Vector3 startPos;
    private bool pickedUp = false;

    void Start()
    {
        startPos = visualObject.transform.position;
    }

    void Update()
    {
        visualObject.transform.Rotate(rotationSpeed * Time.deltaTime);

        float newY = startPos.y + Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude;
        visualObject.transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (pickedUp) return;
            pickedUp = true;
            PickupMessage.instance.Show(pickupName, pickupDescription);
            SoundFXManager.instance.PlaySoundFxClip(pickupSound, transform, volume);
            Pickup();
        }
    }

    public virtual void Pickup()
    {
        throw new System.NotImplementedException();
    }
}
