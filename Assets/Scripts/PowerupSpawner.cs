using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerupSpawner : MonoBehaviour
{
    [SerializeField] private List<SpawnablePowerup> powerups;
    
    public void SpawnRandomPowerup()
    {
        if (powerups == null || powerups.Count == 0)
        {
            Debug.LogWarning("No powerups available to spawn.");
            return;
        }

        int totalWeight = 0;
        foreach (var powerup in powerups)
        {
            totalWeight += powerup.weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentSum = 0;

        foreach (var powerup in powerups)
        {
            currentSum += powerup.weight;
            if (randomValue < currentSum)
            {
                if (PowerupManager.instance.PowerupAlreadyActive(powerup.powerupType) && powerup.canSpawnOnlyOnce)
                {
                    continue;
                }
                Instantiate(powerup.powerupPrefab, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), Quaternion.identity);
                return;
            }
        }
    }

    private void Start()
    {
        SpawnRandomPowerup();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}

[System.Serializable]
public class SpawnablePowerup
{
    public PowerupManager.PowerupType powerupType;
    public GameObject powerupPrefab;
    public int weight;
    public bool canSpawnOnlyOnce = false;
}
