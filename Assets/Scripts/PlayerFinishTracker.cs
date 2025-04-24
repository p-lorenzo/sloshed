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
    
    [Header("Winscreen Elements")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winText;
    public bool win = false;
    
    [Header("Loosescreen Elements")]
    [SerializeField] private GameObject losePanel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            activeFinishTriggers.Add(other);
            if (GameManager.instance.isFallen)
            {
                CheckWin();
            }
        }
    }

    private void WinGame()
    {
        if (win) return;
        winPanel.SetActive(true);
        win = true;
        int level = GameManager.instance.currentLevel;
        string timesText = level == 1 ? "time" : "times";
        winText.text = $"You made it to the Bed!\n{level} {timesText}!";
        GameManager.instance.AddDepthOfField();
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
        hud.SetActive(false);
        if (activeFinishTriggers.Count > 0 || win)
        {
            WinGame();
            return;
        }        
        GameManager.instance.AddDepthOfField();
        losePanel.SetActive(true);
    }

    public void CheckWin()
    {
        if (activeFinishTriggers.Count > 0 || win && !losePanel.activeSelf)
        {
            GameManager.instance.behaviourPuppet.canGetUp = false;
            GameManager.instance.finished = true;
            CursorManager.instance.UnlockCursor();
            GameManager.instance.puppetMaster.state = PuppetMaster.State.Dead;
            hud.SetActive(false);
            StartCoroutine(WinGameAfterDelay());
            return;
        }
    }
    
    private IEnumerator WinGameAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        WinGame();
    }
}