using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PowerupManager : MonoBehaviour
{
    public enum PowerupType {Flashlight, GetterUpper, Water, Dizzimeter};

    private ParticleSystem pickupEffect;

    public static PowerupManager instance;

    public Dictionary<PowerupType, int> activePowerups = new Dictionary<PowerupType, int>();
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebindReferences();
    }    
    
    private void RebindReferences()
    {
        pickupEffect = GameObject.Find("PowerupEffect").GetComponent<ParticleSystem>();
    }

    public void FlashlightPowerup()
    {
        DrunkEffectController.instance.FlashlightPowerup();
        AddPowerup(PowerupType.Flashlight);
    }

    public void GetterUpperPowerup()
    {
        GameManager.instance.AddGetterUpper();
        AddPowerup(PowerupType.GetterUpper);
    }

    public void UseGetterUpperPowerup()
    {
        UsePowerup(PowerupType.GetterUpper);
    }

    public void WaterPowerup()
    {
        DrunkEffectController.instance.DrinkWater();
        AddPowerup(PowerupType.Water);
    }

    public void DizzimeterPowerup()
    {
        DrunkEffectController.instance.DizzimeterPowerup();
        AddPowerup(PowerupType.Dizzimeter);
    }

    private void AddPowerup(PowerupType type)
    {
        pickupEffect.Play();
        
        if (activePowerups.ContainsKey(type))
        {
            activePowerups[type]++;
        }
        else
        {
            activePowerups[type] = 1;
        }
    }
    
    private void UsePowerup(PowerupType type)
    {
        if (activePowerups.ContainsKey(type))
        {
            activePowerups[type]--;
        }
    }

    public bool PowerupAlreadyActive(PowerupType powerupType)
    {
        foreach (var powerup in activePowerups)
        {
            Debug.Log(powerup.Key + ": " + powerup.Value);
        }
        return activePowerups.ContainsKey(powerupType);
    }

    public void RestartLevel()
    {
        activePowerups.Clear();
    }

    public bool HasAtLeastOneGetterUpper()
    {
        if (activePowerups.ContainsKey(PowerupType.GetterUpper)) return activePowerups[PowerupType.GetterUpper] > 0;
        return false;
    }
}
