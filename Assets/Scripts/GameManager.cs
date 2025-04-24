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
    [SerializeField] private ThirdPersonController thirdPersonController;

    [Header("Game Timer")] 
    public bool finished = false;
    public bool started = false;

    private PlayerFinishTracker _playerFinishTracker;

    [Header("Endgame blur")]
    private DepthOfField _depthOfField;

    [Header("Progression")] 
    public int currentLevel = 0;

    [Header("Get ups")] 
    [SerializeField] public int getUpsCount = 0;
    [SerializeField] public bool isFallen = false;

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
        finished = false;
        started = false;
        RebindReferences();
        levelCounter.text = currentLevel.ToString();        
        UpdateGetterUpperCounter();
        runtimeDungeon.Generator.LengthMultiplier = 1f + (currentLevel * 0.25f);
        runtimeDungeon.Generator.DungeonFlow.BranchCount = new IntRange(1, Mathf.RoundToInt(1 + (currentLevel * 0.25f)));
        runtimeDungeon.Generator.Generate();
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
    }

    public void Fallen()
    {
        if (isFallen) return;
        isFallen = true;
        Debug.Log("Fallen");
        if (getUpsCount > 0)
        {
            _playerFinishTracker.CheckWin();
            thirdPersonController.UnDive();
            return;
        }
        Debug.Log("Fallen for good");
        behaviourPuppet.canGetUp = false;
        finished = true;
        CursorManager.instance.UnlockCursor();
        puppetMaster.state = PuppetMaster.State.Dead;
        StartCoroutine(EndAfterDelay());
    }

    public void GetUp()
    {
        PowerupManager.instance.UseGetterUpperPowerup();
        getUpsCount -= 1;
        UpdateGetterUpperCounter();
        isFallen = false;
        thirdPersonController.UnDive();
    }
    
    private IEnumerator EndAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        _playerFinishTracker.EndGame();
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
        getUpsCount = 0;
        isFallen = false;
    }

    public void AddGetterUpper()
    {
        getUpsCount += 1;
        UpdateGetterUpperCounter();
    }

    private void UpdateGetterUpperCounter()
    {
        if (getUpsCount == 0)
        {
            getterUpperCounter.text = "";
            return;
        }
        
        getterUpperCounter.text = $"Getter uppers: {getUpsCount.ToString()}";
    }

    public void EndGame()
    {
        Debug.Log("Fallen for good");
        behaviourPuppet.canGetUp = false;
        finished = true;
        CursorManager.instance.UnlockCursor();
        puppetMaster.state = PuppetMaster.State.Dead;
        _playerFinishTracker.EndGame();
    }
}
