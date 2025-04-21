using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public abstract class Powerup : MonoBehaviour, Pickupable
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            Pickup();
    }

    public virtual void Pickup()
    {
        throw new System.NotImplementedException();
    }
}
