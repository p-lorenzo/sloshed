using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PowerupManager : MonoBehaviour
{
    public enum PowerupType {Flashlight, GetterUpper, Water, Dizzimeter, GuideLight};

    private GuideLightSpawner guideLightSpawner;
    private FlashlightManager flashlight;
    
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
        LoadPowerups();

    }
    
    private void LoadPowerups()
    {
        foreach (PowerupType powerupType in System.Enum.GetValues(typeof(PowerupType)))
        {
            activePowerups.Add(powerupType, PlayerPrefs.GetInt(powerupType.ToString(), 0));
            Debug.Log($"Load {powerupType}: {PlayerPrefs.GetInt(powerupType.ToString())}");
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
        SavePowerups();
    }

    private void SavePowerups()
    {
        foreach (PowerupType powerupType in System.Enum.GetValues(typeof(PowerupType)))
        {
            if (!activePowerups.ContainsKey(powerupType)) return;
            PlayerPrefs.SetInt(powerupType.ToString(), activePowerups[powerupType]);
            Debug.Log($"Save {powerupType}: {PlayerPrefs.GetInt(powerupType.ToString())}");
        }
    }
    
    private void RebindReferences()
    {
        pickupEffect = GameObject.Find("PowerupEffect").GetComponent<ParticleSystem>();
        guideLightSpawner = GameObject.FindWithTag("PlayerController").GetComponent<GuideLightSpawner>();
        flashlight = GameObject.FindWithTag("Flashlight").GetComponent<FlashlightManager>();
    }

    public void FlashlightPowerup()
    {
        flashlight.ActivateFlashlight();
        AddPowerup(PowerupType.Flashlight);
    }

    public void GetterUpperPowerup()
    {
        AddPowerup(PowerupType.GetterUpper);
        GameManager.instance.AddGetterUpper();
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

    public void GuideLightPowerup()
    {
        guideLightSpawner.isPowerupActive = true;
        AddPowerup(PowerupType.GuideLight);
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
    
    public void UsePowerup(PowerupType type)
    {
        if (activePowerups.ContainsKey(type))
        {
            activePowerups[type]--;
        }
    }

    public bool PowerupAlreadyActive(PowerupType powerupType)
    {
        return activePowerups.ContainsKey(powerupType);
    }

    public void RestartLevel()
    {
        activePowerups.Clear();
        ClearSave();
    }

    private void ClearSave()
    {
        foreach (PowerupType powerupType in System.Enum.GetValues(typeof(PowerupType)))
        {
            PlayerPrefs.SetInt(powerupType.ToString(), 0);
        }
    }
    
    public bool HasAtLeastOnePowerUpOfType(PowerupType powerupType)
    {
        if (activePowerups.TryGetValue(powerupType, out var powerup)) return powerup > 0;
        return false;
    }

    public int HowManyPowerupsOfType(PowerupType powerupType)
    {
        return activePowerups.GetValueOrDefault(powerupType, 0);
    }
}
