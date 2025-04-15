using System;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class DrunkEffectController : MonoBehaviour
{
    [Header("Camera Noise Settings")]
    [SerializeField] private  CinemachineBasicMultiChannelPerlin cameraNoise;
    [SerializeField] private  float cameraNoiseDrunknessMultiplier = 1.5f;
    
    [Header("References")]
    [SerializeField] private  CharacterController controller;
    [SerializeField] private  ThirdPersonController thirdPersonController;
    [SerializeField] private  Material drunkMat;
    [SerializeField] private  TextMeshProUGUI drunknessMeter;
    
    [Header("Drunkness Settings")]
    [Range(0f, .5f)] [SerializeField] private  float drunkLevel = 0f;
    [SerializeField] private  float drunkGainRate = 0.01f;
    [SerializeField] private  float drunkDecayRate = 0.01f;
    
    [Header("Player Drift Settings")]
    private Vector3 targetDrift;
    private Vector3 currentDrift;
    [SerializeField] private  float driftMagnitude = 1.2f;
    private float driftCooldown = 3f;
    private float driftTimer = 0f;

    [Header("Player Tilt Reference")] 
    [SerializeField] private DrunkPuppetTilt playerTilt;
    
    void Update()
    {
        int drunknessPercent = Mathf.RoundToInt((drunkLevel / 0.5f) * 100f);
        cameraNoise.AmplitudeGain = 1 + drunkLevel * cameraNoiseDrunknessMultiplier;
        cameraNoise.FrequencyGain = 1 + drunkLevel * cameraNoiseDrunknessMultiplier;
        drunknessMeter.text = $"Drunkness: {drunknessPercent}%";
        
        if (controller.velocity.magnitude < 0.1f)
        {
            drunkLevel -= drunkDecayRate * Time.deltaTime;
        } else if (controller.velocity.magnitude > 0.1f)
        {
            drunkLevel += drunkGainRate * Time.deltaTime;
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
        
        drunkMat.SetFloat("_Amplitude", drunkLevel * 0.03f);
        drunkMat.SetFloat("_GhostStrength", drunkLevel);
        drunkMat.SetFloat("_WaveSpeed", Mathf.Lerp(1f, 10f, drunkLevel));
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
}