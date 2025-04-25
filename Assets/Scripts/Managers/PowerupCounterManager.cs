using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DunGen;
using UnityEngine;

public class PowerupCounterManager : MonoBehaviour
{
    [SerializeField] private RuntimeDungeon runtimeDungeon;
    public static PowerupCounterManager instance;
    
    public int powerupCounter;
    private void Awake()
    {
        if (instance == null) instance = this;
        if (runtimeDungeon != null)
        {
            runtimeDungeon.Generator.RegisterPostProcessStep(OnDungeonComplete, priority: 0, phase: PostProcessPhase.AfterBuiltIn);
        }
    }

    public void OnDungeonComplete(DungeonGenerator generator)
    {
        StartCoroutine(CountPowerups());
    }

    private IEnumerator CountPowerups()
    {
        yield return new WaitForSeconds(3f);
        powerupCounter = GameObject.FindGameObjectsWithTag("Powerup").Length;
        Debug.Log($"Found {powerupCounter} powerups");
    }
}
