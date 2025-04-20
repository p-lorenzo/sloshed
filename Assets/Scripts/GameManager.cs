using System;
using System.Collections;
using System.Threading.Tasks;
using RootMotion.Dynamics;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SocialPlatforms.Impl;

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
    
    [Header("Leaderboard")]
    [SerializeField] private TMP_InputField nameInputField;
    
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

    public void RemoveDepthOfField()
    {
        _depthOfField.active = false;
    }

    private void Update()
    {
        if (!finished && started)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("0.00 s");
        }
    }

    public void LeaderboardSend()
    {
        Debug.Log($"Leaderboard Send {nameInputField.text} in {timer}");
        StartCoroutine(SendScore(nameInputField.text, timer));
    }
    
    IEnumerator SendScore(string leaderboardName, float time)
    {
        string url = "http://dreamlo.com/lb/Arj4UvPYVkmX5z2QsHIx8QhbjfOPFOZ0a_duZaffDvIA/add/" + UnityWebRequest.EscapeURL(leaderboardName) + "/" + Mathf.FloorToInt(time*100.0f);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (!www.result.Equals(UnityWebRequest.Result.Success))
                Debug.Log("Errore invio: " + www.error);
        }
    }
}
