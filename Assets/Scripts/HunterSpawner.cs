using System;
using System.Collections.Generic;
using System.Linq;
using DunGen;
using JetBrains.Annotations;
using UnityEngine;

public class HunterSpawner : MonoBehaviour
{
    public static HunterSpawner instance;
    
    [SerializeField] private RuntimeDungeon runtimeDungeon;
    [SerializeField] private AudioClip spawnCue;
    [SerializeField] private float spawnCueVolume = 1f;
    [SerializeField] private GameObject hunterPrefab;
    [SerializeField] private Transform player;

    private List<GameObject> spawnPoints = new();
    private float timeSinceSessionStart;
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
        if (GameManager.instance.currentLevel <= 3) return;
        if (spawnPoints.Count <= 3) return;
        if (enemyHasSpawned) return;
        timeSinceSessionStart += Time.deltaTime;
        if (timeSinceSessionStart >= 30f) TimeSpawn();
    }
    
    private IOrderedEnumerable<GameObject> OrderSpawnPoints()
    {
        return spawnPoints.Where(x => x).OrderBy(x => Vector3.Distance(x.transform.position, player.position));
    } 
    
    public void TeddySpawn() => SpawnHunter(OrderSpawnPoints().FirstOrDefault());
    private void TimeSpawn() =>  SpawnHunter(OrderSpawnPoints().LastOrDefault());

    private void SpawnHunter([CanBeNull] GameObject spawnPoint)
    {
        if (!spawnPoint) return;
        SoundFXManager.instance.PlaySoundFxClip(spawnCue, player.transform, spawnCueVolume);
        Instantiate(hunterPrefab, spawnPoint.transform.position, Quaternion.identity);
        enemyHasSpawned = true;
    }
    
    
    
}
