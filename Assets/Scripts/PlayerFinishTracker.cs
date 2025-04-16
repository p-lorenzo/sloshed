using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerFinishTracker : MonoBehaviour
{
    private readonly HashSet<Collider> activeFinishTriggers = new();

    private GameManager _gameManager;
    [Header("Winscreen Elements")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winText;
    private float winTimer = 0f;
    public bool win = false;
    
    [Header("Loosescreen Elements")]
    [SerializeField] private GameObject losePanel;

    private void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            activeFinishTriggers.Add(other);
            if (_gameManager.finished)
            {
                WinGame();
            }
        }
    }

    private void WinGame()
    {
        if (win) return;
        winTimer = _gameManager.timer;
        winPanel.SetActive(true);
        winText.text = "It only took you " + winTimer.ToString("0.00") + " seconds.";
        win = true;
        _gameManager.AddDepthOfField();
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            activeFinishTriggers.Remove(other);
        }
    }
    
    public void EndGame()
    {
        if (activeFinishTriggers.Count > 0 || win)
        {
            WinGame();
            return;
        }        
        _gameManager.AddDepthOfField();
        losePanel.SetActive(true);
    }
}