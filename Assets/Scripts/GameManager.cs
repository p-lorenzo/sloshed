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
    [SerializeField] private BehaviourPuppet behaviourPuppet;
    [SerializeField] private PuppetMaster puppetMaster;
    [SerializeField] private RuntimeDungeon runtimeDungeon;
    [SerializeField] private TextMeshProUGUI levelCounter;
    
    [Header("Game Timer")]
    public bool finished = false;
    public bool started = false;
    
    private PlayerFinishTracker _playerFinishTracker;
    
    [Header("Endgame blur")]
    private DepthOfField _depthOfField;
    
    [Header("Progression")]
    public int currentLevel = 0;

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
        runtimeDungeon.Generator.LengthMultiplier = 1f + (currentLevel * 0.25f);
        runtimeDungeon.Generator.Generate();
    }
    
    private void RebindReferences()
    {
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
    }

    public void Fallen()
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
    }

    public void Retry()
    {
        currentLevel = 0;
    }
}
