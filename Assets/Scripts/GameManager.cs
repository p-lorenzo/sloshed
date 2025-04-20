using System;
using System.Collections;
using System.Threading.Tasks;
using RootMotion.Dynamics;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    [Header("Game Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    public float timer;
    public bool finished = false;
    public bool started = false;
    
    private PlayerFinishTracker _playerFinishTracker;
    
    private CursorManager cursorManager;
    
    [Header("Endgame blur")]
    [SerializeField] private Volume globalVolume;
    private DepthOfField _depthOfField;
    
    [Header("Puppet stuff")]
    [SerializeField] private BehaviourPuppet behaviourPuppet;
    [SerializeField] private PuppetMaster puppetMaster;
    private void Start()
    {
        _playerFinishTracker = FindFirstObjectByType<PlayerFinishTracker>();
        globalVolume.profile.TryGet<DepthOfField>(out _depthOfField);
        cursorManager = FindAnyObjectByType<CursorManager>();
    }

    public void Fallen()
    {
        finished = true;
        cursorManager.UnlockCursor();
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

    private void Update()
    {
        if (!finished && started)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("0.00 s");
        }
    }
}
