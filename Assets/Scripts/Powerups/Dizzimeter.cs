using System;
using UnityEngine;

public class Dizzimeter : Powerup
{
    override public void Pickup()
    {
        DrunkEffectController.instance.DizzimeterPowerup();
        Destroy(gameObject);
    }
}
