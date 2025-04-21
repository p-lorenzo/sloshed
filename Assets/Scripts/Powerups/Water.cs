using System;
using UnityEngine;

public class Water : Powerup
{
    override public void Pickup()
    {
        DrunkEffectController.instance.DrinkWater();
        Destroy(gameObject);
    }
}
