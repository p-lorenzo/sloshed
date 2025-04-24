using System;
using UnityEngine;

public class GuideLight : Powerup
{
    public override void Pickup()
    {
        PowerupManager.instance.GuideLightPowerup();
        Destroy(gameObject);
    }
}