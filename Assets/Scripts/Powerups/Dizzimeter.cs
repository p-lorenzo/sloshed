using System;
using UnityEngine;

public class Dizzimeter : Powerup
{
    [SerializeField] private string pickupName;
    [SerializeField] private string pickupDescription;

    public override void Pickup()
    {
        DrunkEffectController.instance.DizzimeterPowerup();
        PickupMessage.instance.Show(pickupName, pickupDescription);
        Destroy(gameObject);
    }
}
