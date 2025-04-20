using System;
using System.Collections;
using System.Threading.Tasks;
using LootLocker.Requests;
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
    [SerializeField] private GameObject leaderboardInsertScorePanel;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject leaderboardButton;
    [SerializeField] private GameObject winPanel;
    string leaderboardKey = "30765";
    
    private void Start()
    {
        _playerFinishTracker = FindFirstObjectByType<PlayerFinishTracker>();
        globalVolume.profile.TryGet<DepthOfField>(out _depthOfField);
        cursorManager = FindAnyObjectByType<CursorManager>();
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (!response.success)
            {
                Debug.Log(response.text);

                return;
            }

            Debug.Log("successfully started LootLocker session");
        });
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
        SendScore(nameInputField.text, timer);
        leaderboardInsertScorePanel.SetActive(false);
        leaderboardButton.SetActive(true);
    }
    
    private void SendScore(string leaderboardName, float time)
    {
        LootLockerSDKManager.SetPlayerName(leaderboardName, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Player name updated!");
            }
            else
            {
                Debug.Log("Failed to update player name.");
            }
        });
        LootLockerSDKManager.SubmitScore(leaderboardName, (int)(time*100f), leaderboardKey, (response) =>
        {
            if (!response.success)
            {
                Debug.Log("Could not submit score!");
                Debug.Log(response.errorData.ToString());
            }
        });
    }

    private void FetchLeaderboard()
    {
        int count = 50;

        LootLockerSDKManager.GetScoreList(leaderboardKey, count, 0, (response) =>
        {
            if (!response.success) {
                Debug.Log("Could not get score list!");
                Debug.Log(response.errorData.ToString());
                return;
            } 
            foreach (var entry in response.items)
            {
                Debug.Log($"{entry.rank}. {entry.player.name} - {entry.score}");
            }
            Debug.Log("Successfully got score list!");
        });
    }

    public void ShowLeaderboard()
    {
        FetchLeaderboard();
        winPanel.SetActive(false);
        leaderboardPanel.SetActive(true);
    }
}
