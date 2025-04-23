using System;
using UnityEngine;

public class Flashlight : Powerup
{
    public override void Pickup()
    {
        DrunkEffectController.instance.FlashlightPowerup();
        Destroy(gameObject);
    }
}