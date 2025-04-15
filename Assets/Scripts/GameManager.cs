using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float timer;
    private bool finished = false;
    
    private PlayerFinishTracker _playerFinishTracker;

    public GameObject winPanel;
    private float winTimer = 0f;
    public TextMeshProUGUI winText;
    
    public GameObject losePanel;
    AudioSource audioData;
    
    public Volume globalVolume;
    private DepthOfField _depthOfField;
    private void Start()
    {
        _playerFinishTracker = FindFirstObjectByType<PlayerFinishTracker>();
        globalVolume.profile.TryGet<DepthOfField>(out _depthOfField);
    }

    public void Fallen()
    {
        finished = true;
        _depthOfField.active = true;
        if (_playerFinishTracker != null && _playerFinishTracker.IsOnBed())
        {
            Debug.Log("Fallen On Bed!");
            winTimer = timer;
            winPanel.SetActive(true);
            winText.text = "It only took you " + winTimer.ToString("0.00") + " seconds.";
        }
        else
        {
            Debug.Log("Fallen not on Bed!");
            losePanel.SetActive(true);
            audioData = GetComponent<AudioSource>();
            audioData.Play(0);
        }
    }

    public void Update()
    {
        if (!finished)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("0.00 s");
        }
    }
}
