using System;
using UnityEngine;

public class Flashlight : Powerup
{
    public override void Pickup()
    {
        PowerupManager.instance.FlashlightPowerup();
        Destroy(gameObject);
    }
}