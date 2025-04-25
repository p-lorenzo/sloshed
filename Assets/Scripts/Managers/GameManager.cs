using System;
using System.Collections;
using DunGen;
using System.Collections.Generic;
using System.Threading.Tasks;
using RootMotion.Dynamics;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] public BehaviourPuppet behaviourPuppet;
    [SerializeField] public PuppetMaster puppetMaster;
    [SerializeField] private RuntimeDungeon runtimeDungeon;
    [SerializeField] private TextMeshProUGUI levelCounter;
    [SerializeField] private TextMeshProUGUI getterUpperCounter;
    [SerializeField] private TextMeshProUGUI holyWaterCounter;
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private HolyWaterManager holyWaterManager;

    [Header("Game Timer")] 
    public bool finished = false;
    public bool started = false;

    private PlayerFinishTracker _playerFinishTracker;

    [Header("Endgame blur")]
    private DepthOfField _depthOfField;

    [Header("Progression")] 
    public int currentLevel = 0;

    public bool isFallen = false;

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
        LoadSave();
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
        StopAllCoroutines();
        finished = false;
        started = false;
        RebindReferences();
        levelCounter.text = currentLevel.ToString();
        SaveGame();
        UpdateGetterUpperCounter();
        UpdateHolyWaterCounter();
        thirdPersonController.CheckPowerups();

        switch (currentLevel)
        {
            case 1:
                runtimeDungeon.Generator.DungeonFlow.Length = new IntRange(3, 3);
                runtimeDungeon.Generator.DungeonFlow.BranchCount = new IntRange(0, 1);
                break;
            case > 1:
                var dungeonMax = Mathf.RoundToInt(3 + currentLevel * 0.25f);
                runtimeDungeon.Generator.DungeonFlow.Length = new IntRange(dungeonMax, dungeonMax + 3);
                runtimeDungeon.Generator.DungeonFlow.BranchCount = new IntRange(1, 3);
                break;
        }
        
        runtimeDungeon.Generator.Generate();
    }

    private void SaveGame()
    {
        PlayerPrefs.SetInt("currentLevel", currentLevel);
        Debug.Log($"Saved level: {PlayerPrefs.GetInt("currentLevel")}");
    }

    private void LoadSave()
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel", 1);
        Debug.Log($"Loaded level: {PlayerPrefs.GetInt("currentLevel")}");
    }
    
    private void RebindReferences()
    {
        if (thirdPersonController == null)
            thirdPersonController = FindAnyObjectByType<ThirdPersonController>();
            
        if (globalVolume == null)
            globalVolume = FindAnyObjectByType<Volume>();

        if (behaviourPuppet == null)
        {
            behaviourPuppet = FindAnyObjectByType<BehaviourPuppet>();
            if (behaviourPuppet.onLoseBalance.unityEvent != null)
            {
                behaviourPuppet.onLoseBalance.unityEvent.AddListener(Fallen);
            }
            else
            {
                Debug.LogWarning("onLoseBalance or unityEvent is null!");
            }
        }

        if (puppetMaster == null)
            puppetMaster = FindAnyObjectByType<PuppetMaster>();

        if (runtimeDungeon == null)
            runtimeDungeon = FindAnyObjectByType<RuntimeDungeon>();

        if (globalVolume != null)
            globalVolume.profile.TryGet(out _depthOfField);
            
        if (_playerFinishTracker == null)
            _playerFinishTracker = FindFirstObjectByType<PlayerFinishTracker>();
        
        if (levelCounter == null)
            levelCounter = GameObject.Find("RoundCounter").GetComponent<TextMeshProUGUI>();        
        
        if (getterUpperCounter == null)
            getterUpperCounter = GameObject.Find("GetterUppers").GetComponent<TextMeshProUGUI>();
        
        if (holyWaterCounter == null)
            holyWaterCounter = GameObject.Find("HolyWaters").GetComponent<TextMeshProUGUI>();

        if (holyWaterManager == null)
            holyWaterManager = GameObject.Find("HolyWaterGameObject").GetComponent<HolyWaterManager>();
    }

    public void Fallen()
    {
        if (isFallen) return;
        isFallen = true;
        Debug.Log("Fallen");
        if (PowerupManager.instance.HowManyPowerupsOfType(PowerupManager.PowerupType.GetterUpper) > 0)
        {
            if (_playerFinishTracker.CheckWin())
            {
                
            }
            thirdPersonController.UnDive();
            return;
        }
        Debug.Log("Fallen for good");
        behaviourPuppet.canGetUp = false;
        FinishRound();
    }

    public void FinishRound()
    {
        finished = true;
        CursorManager.instance.UnlockCursor();
        puppetMaster.state = PuppetMaster.State.Dead;
        StartCoroutine(EndAfterDelay());
    }
    
    private IEnumerator EndAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        _playerFinishTracker.EndGame();
    }

    public void GetUp()
    {
        PowerupManager.instance.UseGetterUpperPowerup();
        UpdateGetterUpperCounter();
        isFallen = false;
        thirdPersonController.UnDive();
    }

    public void UseHolyWater()
    {
        if (!PowerupManager.instance.HasAtLeastOnePowerUpOfType(PowerupManager.PowerupType.HolyWater)) return;
        PowerupManager.instance.UseHolyWaterPowerup();
        UpdateHolyWaterCounter();
        holyWaterManager.OnItemUsed();
    }
    
    public void AddDepthOfField()
    {
        _depthOfField.active = true;
    }

    public void RemoveDepthOfField()
    {
        _depthOfField.active = false;
    }

    public void NextLevel()
    {
        currentLevel++;
        isFallen = false;
    }

    public void Retry()
    {
        currentLevel = 1;
        ClearSave();
        isFallen = false;
    }

    private void ClearSave()
    {
        PlayerPrefs.SetInt("currentLevel", 1);
    }

    public void AddGetterUpper()
    {
        UpdateGetterUpperCounter();
    }

    private void UpdateGetterUpperCounter()
    {
        var getterUppers = PowerupManager.instance.HowManyPowerupsOfType(PowerupManager.PowerupType.GetterUpper);
        if (getterUppers == 0)
        {
            getterUpperCounter.text = "";
            return;
        }
        
        getterUpperCounter.text = $"Getter uppers: {getterUppers}";
    }
    
    public void AddHolyWater()
    {
        UpdateHolyWaterCounter();
    }

    private void UpdateHolyWaterCounter()
    {
        var holyWaters = PowerupManager.instance.HowManyPowerupsOfType(PowerupManager.PowerupType.HolyWater);
        if (holyWaters == 0)
        {
            holyWaterCounter.text = "";
            return;
        }
        
        holyWaterCounter.text = $"Holy water: {holyWaters}";
    }
}
