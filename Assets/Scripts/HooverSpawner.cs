using System;
using System.Collections.Generic;
using System.Linq;
using DunGen;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class HooverSpawner : MonoBehaviour
{
    public static HooverSpawner instance;
    
    [SerializeField] private RuntimeDungeon runtimeDungeon;
    [SerializeField] private GameObject hooverPrefab;
    [SerializeField] private Transform player;

    private List<GameObject> spawnPoints = new();
    public bool enemyHasSpawned;
    
    private void Awake()
    {
        if (instance == null) instance = this;
        if (runtimeDungeon != null)
        {
            runtimeDungeon.Generator.RegisterPostProcessStep(OnPostProcess, 0, PostProcessPhase.AfterBuiltIn);
        }
    }

    private void OnPostProcess(DungeonGenerator dungeonGenerator)
    {
        spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("EnemySpawner"));
    }
    
    void Update()
    {
        if (!CheckSpawnConditions()) return;
        if (enemyHasSpawned) return;
        if (RandomSpawn()) SpawnHoover(OrderSpawnPoints().LastOrDefault());
    }

    private bool RandomSpawn()
    {
        return Random.Range(0,1) % 2 == 0;
    }

    private bool CheckSpawnConditions()
    {
        if (!PowerupManager.instance.HasAtLeastOnePowerUpOfType(PowerupManager.PowerupType.GetterUpper)) return false;
        if (GameManager.instance.currentLevel <= 3) return false;
        if (spawnPoints.Count <= 3) return false;
        return true;
    }
    
    private IOrderedEnumerable<GameObject> OrderSpawnPoints()
    {
        return spawnPoints.Where(x => x).OrderBy(x => Vector3.Distance(x.transform.position, player.position));
    } 
    
    private void SpawnHoover([CanBeNull] GameObject spawnPoint)
    {
        if (!spawnPoint) return;
        Instantiate(hooverPrefab, spawnPoint.transform.position, Quaternion.identity);
        enemyHasSpawned = true;
    }
    
    
    
}
