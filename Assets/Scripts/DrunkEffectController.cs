using System;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class DrunkEffectController : MonoBehaviour
{
    public CinemachineBasicMultiChannelPerlin cameraNoise;
    public float cameraNoiseDrunknessMultiplier = 1.5f;
    public CharacterController controller;
    public ThirdPersonController thirdPersonController;
    public Material drunkMat;
    public TextMeshProUGUI drunknessMeter;
    [Range(0f, .5f)] public float drunkLevel = 0f;
    public float drunkGainRate = 0.01f;
    public float drunkDecayRate = 0.01f;
    private Vector3 targetDrift;
    private Vector3 currentDrift;
    public float driftMagnitude = 1.2f;
    private float driftCooldown = 3f;
    private float driftTimer = 0f;

    [Header("Player Tilt Settings")] 
    public bool isTiltingEnabled = true;
    public Transform playerVisual; // riferimento alla mesh/visivo, NON il root
    public float maxTiltAngle = 20f;
    public float tiltSpeed = .2f;
    public float tiltMagnitude = 2f;

    
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
        currentDrift = Vector3.Lerp(currentDrift, targetDrift, Time.deltaTime * tiltSpeed);
        thirdPersonController.InputOffset = currentDrift * (drunkLevel * driftMagnitude);
        
        drunkLevel = Mathf.Clamp(drunkLevel, 0f, .5f);
        
        if (playerVisual && targetDrift != Vector3.zero && isTiltingEnabled)
        {
            Vector3 localDrift = transform.InverseTransformDirection(targetDrift);

            float tiltX = -localDrift.z * drunkLevel * maxTiltAngle * tiltMagnitude;
    
            float tiltZ = localDrift.x * drunkLevel * maxTiltAngle * tiltMagnitude;

            Quaternion targetTilt = Quaternion.Euler(tiltX, 0f, tiltZ);

            playerVisual.localRotation = Quaternion.Slerp(
                playerVisual.localRotation,
                targetTilt,
                Time.deltaTime * tiltSpeed
            );
        }
        
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