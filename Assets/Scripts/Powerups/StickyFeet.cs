using System;
using UnityEngine;

public class StickyFeet : Powerup
{
    public override void Pickup()
    {
        PowerupManager.instance.StickyFeetPowerup();
        Destroy(gameObject);
    }
}
