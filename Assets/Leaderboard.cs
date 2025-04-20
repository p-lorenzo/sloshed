using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private GameObject scoreEntryPrefab;
    [SerializeField] private Transform leaderboardTableParent;

    public void InitializeLeaderboard(List<LeaderboardEntry> leaderboard)
    {
        foreach (var leaderboardEntry in leaderboard)
        {
            GameObject scoreEntry = Instantiate(scoreEntryPrefab, leaderboardTableParent);
            scoreEntry.GetComponent<ScoreEntry>().Initialize($"{leaderboardEntry.rank}.", leaderboardEntry.playerName, $"{leaderboardEntry.time/100.0f}s");
        }
    }
}

public struct LeaderboardEntry
{
    public int rank;
    public string playerName;
    public int time;
    
    public LeaderboardEntry(int rank, string playerName, int time)
    {
        this.rank = rank;
        this.playerName = playerName;
        this.time = time;
    }
}
