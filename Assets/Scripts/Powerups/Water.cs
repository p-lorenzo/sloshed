using System;
using UnityEngine;

public class Water : Powerup
{
    [SerializeField] private string pickupName;
    [SerializeField] private string pickupDescription;
    
    public override void Pickup()
    {
        DrunkEffectController.instance.DrinkWater();
        PickupMessage.instance.Show(pickupName, pickupDescription);
        Destroy(gameObject);
    }
}
