using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DrunkEffectController : MonoBehaviour
{
    public CharacterController controller;
    public ThirdPersonController thirdPersonController;
    public Material drunkMat;
    [Range(0f, .5f)] public float drunkLevel = 0f;
    public float drunkGainRate = 0.01f;
    public float drunkDecayRate = 0.01f;
    private Vector3 targetDrift;
    public float driftMagnitude = 1.2f;
    private float cooldown = 3f;
    private float timer = 0f;

    void Update()
    {
        if (controller.velocity.magnitude < 0.1f)
        {
            drunkLevel -= drunkDecayRate * Time.deltaTime;
        } else if (controller.velocity.magnitude > 0.1f)
        {
            drunkLevel += drunkGainRate * Time.deltaTime;
        }
        
        timer += Time.deltaTime;

        if (timer >= cooldown)
        {
            PickNewDrift();
            timer = 0f;
        }
        
        thirdPersonController.InputOffset = targetDrift * (drunkLevel * driftMagnitude);
        
        drunkLevel = Mathf.Clamp(drunkLevel, 0f, .5f);
        
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