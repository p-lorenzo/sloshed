using System;
using UnityEngine;

public class Flashlight : Powerup
{
    [SerializeField] private string pickupName;
    [SerializeField] private string pickupDescription;
    
    public override void Pickup()
    {
        DrunkEffectController.instance.FlashlightPowerup();
        PickupMessage.instance.Show(pickupName, pickupDescription);
        Destroy(gameObject);
    }
}