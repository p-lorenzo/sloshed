using UnityEngine;

public class HolyWater : Powerup
{
    public override void Pickup()
    {
        PowerupManager.instance.HolyWaterPowerup();
        PowerupManager.instance.WaterPowerup();
        Destroy(gameObject);
    }
}
