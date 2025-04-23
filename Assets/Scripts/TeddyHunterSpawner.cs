using System;
using UnityEngine;

public class TeddyHunterSpawner : MonoBehaviour
{
    [SerializeField] private float spawnTimer;
    
    private float deathTimer;

    private void OnTriggerStay(Collider other)
    {
        if (HunterSpawner.instance.enemyHasSpawned) return;
        if (!other.CompareTag("PlayerHead")) return;
        deathTimer += Time.deltaTime;
        if (deathTimer >= spawnTimer) HunterSpawner.instance.TeddySpawn();
    }
}
