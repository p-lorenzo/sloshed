using System;
using UnityEngine;

public class SpeedDemon : Powerup
{
    public override void Pickup()
    {
        PowerupManager.instance.SpeedDemonPowerup();
        Destroy(gameObject);
    }
}
