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

    public void OnDungeonComplete(DungeonGenerator generator)
    {        
        if (!isCounterActive) return;

        StartCoroutine(WaitSeconds(1f));
        CountPowerups();
    }

    private IEnumerator WaitSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    private void CountPowerups()
    {
        powerupCounter = GameObject.FindGameObjectsWithTag("Powerup").Length;
        Debug.Log($"Found {powerupCounter} powerups");
        powerupCounterText.text = $"Powerup left: {powerupCounter}";
    }

    public void UpdateCounter()
    {
        if (!isCounterActive) return;
        CountPowerups();
        powerupCounterText.text = $"Powerup left: {powerupCounter}";
    }
}
