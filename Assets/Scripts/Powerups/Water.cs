using System;
using UnityEngine;

public class Water : Powerup
{
    public override void Pickup()
    {
        PowerupManager.instance.WaterPowerup();
        Destroy(gameObject);
    }
}
