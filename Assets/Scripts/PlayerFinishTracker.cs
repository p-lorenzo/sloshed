using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;
using TMPro;

public class PlayerFinishTracker : MonoBehaviour
{
    private readonly HashSet<Collider> activeFinishTriggers = new();

    [Header("References")] 
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private GameObject losePanel;

    private bool win = false;
    private bool lost = false;

        
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            activeFinishTriggers.Remove(other);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            activeFinishTriggers.Add(other);
            if (GameManager.instance.isFallen && !win && !lost)
            {
                WinGame();
                GameManager.instance.puppetMaster.state = PuppetMaster.State.Dead;
            }
        }
    }

    private void WinGame()
    {
        if (win || lost) return;
        winPanel.SetActive(true);
        win = true;
        int level = GameManager.instance.currentLevel;
        string timesText = level == 1 ? "time" : "times";
        winText.text = $"You made it to the Bed!\n{level} {timesText}!";
        GameManager.instance.AddDepthOfField();
        StopAllCoroutines();
    }

    private void LoseGame()
    {
        if (lost || win) return;
        losePanel.SetActive(true);
        lost = true;
        GameManager.instance.AddDepthOfField();
        StopAllCoroutines();
    }
    
    public void EndGame()
    {
        if (win || lost) return;
        hud.SetActive(false);
        if (activeFinishTriggers.Count > 0)
        {
            WinGame();
            return;
        }        
        LoseGame();
    }

    public bool CheckWin()
    {
        if (win || lost) return false;
        if (activeFinishTriggers.Count > 0)
        {
            WinGame();
            GameManager.instance.puppetMaster.state = PuppetMaster.State.Dead;
            return true;
        }

        return false;
    }
}