using System;
using System.Collections;
using DunGen;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class UILevelManager : MonoBehaviour
{
    public static UILevelManager instance;
    
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Animator animator;
    [SerializeField] private ThirdPersonController controller;
    [SerializeField] private string idleStateName = "Idle";

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        StartCoroutine(WaitForIdleThenFreeze());
    }

    IEnumerator WaitForIdleThenFreeze()
    {
        // Assicurati che il tempo scorra
        Time.timeScale = 1f;

        // Finché l'animator non è nello stato "Idle", aspetta
        while (!IsInIdle())
        {
            yield return null;
        }

        // Dopo un piccolo delay per sicurezza
        yield return new WaitForSecondsRealtime(0.1f);

        Time.timeScale = 0f;
        loadingPanel.SetActive(false);
        GameManager.instance.AddDepthOfField();
        controller.enabled = true;
    }

    bool IsInIdle()
    {
        if (animator == null) return false;
        var state = animator.GetCurrentAnimatorStateInfo(0);
        return state.IsName(idleStateName) && state.normalizedTime >= 0.05f;
    }

    public void StartLevel()
    {
        Time.timeScale = 1;
        startPanel.SetActive(false);
        GameManager.instance.started = true;
        GameManager.instance.RemoveDepthOfField();
    }

    public void RestartLevel()
    {
        DrunkEffectController.instance.RestartLevel();
        GameManager.instance.Retry();
        Time.timeScale = 1f; // utile se il gioco era in pausa
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        GameManager.instance.NextLevel();
        Time.timeScale = 1f; // utile se il gioco era in pausa
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}