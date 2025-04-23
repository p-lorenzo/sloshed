using System;
using UnityEngine;

public class Water : Powerup
{
    public override void Pickup()
    {
        DrunkEffectController.instance.DrinkWater();
        Destroy(gameObject);
    }
}
