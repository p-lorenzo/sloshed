using System;
using UnityEngine;

public class GetterUpper : Powerup
{
    override public void Pickup()
    {
        GameManager.instance.AddGetterUpper();
        Destroy(gameObject);
    }
}
