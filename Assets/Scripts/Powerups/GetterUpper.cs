using System;
using UnityEngine;

public class GetterUpper : Powerup
{
    public override void Pickup()
    {
        PowerupManager.instance.GetterUpperPowerup();
        Destroy(gameObject);
    }
}
