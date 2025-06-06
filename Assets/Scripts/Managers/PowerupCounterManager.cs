using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DunGen;
using TMPro;
using UnityEngine;

public class PowerupCounterManager : MonoBehaviour
{
    [SerializeField] private RuntimeDungeon runtimeDungeon;
    [SerializeField] private TextMeshProUGUI powerupCounterText;
    public static PowerupCounterManager instance;
    public bool isCounterActive;
    
    public int powerupCounter;
    private void Awake()
    {
        if (instance == null) instance = this;
        if (runtimeDungeon != null)
        {
            runtimeDungeon.Generator.RegisterPostProcessStep(OnDungeonComplete, priority: 0, phase: PostProcessPhase.AfterBuiltIn);
        }
    }

    private void Start()
    {
        if (PowerupManager.instance.HasAtLeastOnePowerUpOfType(PowerupManager.PowerupType.PowerupCounter)) isCounterActive = true;
    }

    public void OnDungeonComplete(DungeonGenerator generator)
    {        
        if (!isCounterActive) return;
        StartCoroutine(WaitSeconds(1f));
    }

    private IEnumerator WaitSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        CountPowerups();
    }

    private void CountPowerups()
    {
        powerupCounter = GameObject.FindGameObjectsWithTag("Powerup").Length;
        powerupCounterText.text = $"Powerup left: {powerupCounter}";
    }

    public void UpdateCounter()
    {
        StartCoroutine(WaitSeconds(.5f));
        if (!isCounterActive) return;
        powerupCounterText.text = $"Powerup left: {powerupCounter}";
    }
}
