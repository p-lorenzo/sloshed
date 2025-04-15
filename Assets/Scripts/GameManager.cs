using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    [Header("Game Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    private float timer;
    public bool finished = false;
    public bool started = false;
    
    private PlayerFinishTracker _playerFinishTracker;

    [Header("Winscreen Elements")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winText;
    private float winTimer = 0f;
    
    [Header("Loosescreen Elements")]
    [SerializeField] private GameObject losePanel;
    AudioSource audioData;
    
    [Header("Endgame blur")]
    [SerializeField] private Volume globalVolume;
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

    private void Update()
    {
        if (!finished && started)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("0.00 s");
        }
    }
}
