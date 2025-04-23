using System;
using UnityEngine;

public class Dizzimeter : Powerup
{
    public override void Pickup()
    {
        PowerupManager.instance.DizzimeterPowerup();
        Destroy(gameObject);
    }
}
