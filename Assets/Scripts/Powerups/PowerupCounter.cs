using System;
using UnityEngine;

public class PowerupCounter : Powerup
{
    public override void Pickup()
    {
        PowerupManager.instance.PowerupCounterPowerup();
        Destroy(gameObject);
    }
}
