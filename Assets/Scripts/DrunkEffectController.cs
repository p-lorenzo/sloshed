using System;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class DrunkEffectController : MonoBehaviour
{
    [Header("Camera Noise Settings")] 
    [SerializeField] private CinemachineBasicMultiChannelPerlin cameraNoise;
    [SerializeField] private float cameraNoiseDrunknessMultiplier = 1.5f;

    [Header("References")] 
    [SerializeField] private CharacterController controller;
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private Material drunkMat;
    [SerializeField] private TextMeshProUGUI drunknessMeter;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Volume globalVolume;

    [Header("Audio Settings")] 
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float minCutoffFreq = 100f;
    [SerializeField] private float maxCutoffFreq = 22000f;
    [SerializeField] private AudioSource ringingSound;
    [SerializeField] private float maxRingingSound = 0.01f;
    [SerializeField] private float minRingingSound = 0f;

    [Header("Drunkness Settings")] 
    [Range(0f, .5f)] [SerializeField] private float drunkLevel = 0f;
    [SerializeField] private float drunkGainRate = 0.01f;
    [SerializeField] private float drunkDecayRate = 0.01f;

    [Header("Player Drift Settings")] 
    private Vector3 targetDrift;
    private Vector3 currentDrift;
    [SerializeField] private float driftMagnitude = 1.2f;
    private float driftCooldown = 3f;
    private float driftTimer = 0f;

    [Header("Player Tilt Reference")] 
    [SerializeField] private DrunkPuppetTilt playerTilt;

    [Header("LightBloom Settings")] private Bloom _bloom;

    private void Start()
    {
        if (globalVolume.profile.TryGet(out _bloom))
        {
            _bloom.intensity.value = .5f;
            _bloom.threshold.value = 1.1f;
        }
    }

    void Update()
    {
        if (gameManager.finished)
            return;
        int drunknessPercent = Mathf.RoundToInt((drunkLevel / 0.5f) * 100f);
        cameraNoise.AmplitudeGain = 1 + drunkLevel * cameraNoiseDrunknessMultiplier;
        cameraNoise.FrequencyGain = 1 + drunkLevel * cameraNoiseDrunknessMultiplier;
        drunknessMeter.text = $"Confusion: {drunknessPercent}%";

        if (controller.velocity.magnitude < 0.1f)
        {
            drunkLevel -= drunkDecayRate * Time.deltaTime;
        }
        else if (controller.velocity.magnitude > 0.1f)
        {
            drunkLevel += drunkGainRate * Time.deltaTime * controller.velocity.magnitude;
        }

        driftTimer += Time.deltaTime;

        if (driftTimer >= driftCooldown)
        {
            PickNewDrift();
            driftTimer = 0f;
        }

        currentDrift = Vector3.Lerp(currentDrift, targetDrift, Time.deltaTime);
        thirdPersonController.InputOffset = currentDrift * (drunkLevel * driftMagnitude);

        drunkLevel = Mathf.Clamp(drunkLevel, 0f, .5f);

        playerTilt.drunkLevel = drunkLevel;

        drunkMat.SetFloat("_Amplitude", drunkLevel * 0.01f);
        drunkMat.SetFloat("_GhostStrength", drunkLevel);
        drunkMat.SetFloat("_WaveSpeed", Mathf.Lerp(1f, 10f, drunkLevel));

        audioMixer.SetFloat("_CutoffFreq", DrunknessToAudioValue(maxCutoffFreq, minCutoffFreq));
        ringingSound.volume = DrunknessToAudioValue(minRingingSound, maxRingingSound);
        _bloom.threshold.value = .5f - drunkLevel;
        _bloom.intensity.value = drunkLevel * 50;
    }

    private void OnDestroy()
    {
        drunkMat.SetFloat("_Amplitude", 0f);
        drunkMat.SetFloat("_GhostStrength", 1f);
        drunkMat.SetFloat("_WaveSpeed", 0f);
    }

    private void PickNewDrift()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        targetDrift = new Vector3(randomDir.x, 0, randomDir.y);
    }

    private float DrunknessToAudioValue(float maxValue, float minValue)
    {
        var invertedDrunkLevel = 1f - (drunkLevel / .5f);
        return Mathf.Lerp(minValue, maxValue, Mathf.Pow(invertedDrunkLevel, 2f));
    }
}