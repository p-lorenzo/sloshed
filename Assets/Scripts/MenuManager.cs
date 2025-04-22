using System;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public bool isPaused = false;
    [SerializeField] private GameObject hud;
    public void Awake()
    {
        if (instance == null) instance = this;
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (!GameManager.instance.started || GameManager.instance.finished) return;
        isPaused = true;
        gameObject.SetActive(true);
        hud.SetActive(false);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        isPaused = false;
        gameObject.SetActive(false);
        hud.SetActive(true);
        Time.timeScale = 1;
    }
}
