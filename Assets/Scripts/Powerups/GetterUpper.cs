using System;
using UnityEngine;

public class GetterUpper : Powerup
{
    override public void Pickup()
    {
        PowerupManager.instance.GetterUpperPowerup();
        Destroy(gameObject);
    }
}
