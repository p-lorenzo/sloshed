using System;
using UnityEngine;

public class Dizzimeter : Powerup
{
    public override void Pickup()
    {
        DrunkEffectController.instance.DizzimeterPowerup();
        Destroy(gameObject);
    }
}
