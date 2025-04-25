using UnityEngine;
using RootMotion.Dynamics;
using System.Collections.Generic;

public class DrunkPuppetTilt : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PuppetMaster puppetMaster;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameManager gameManager;

    [Header("Tilt Settings")]
    [SerializeField] private float maxTiltForce = 50f;
    [SerializeField] private float maxPinWeightOffset = 0.5f;
    [SerializeField] private float tiltSmoothing = 5f;
    public float drunkLevel = 0f;

    private readonly string[] targetMuscleNames = { "bip Spine1", "bip Pelvis", "bip Head" };

    private List<int> targetIndices = new();
    private Dictionary<int, float> originalPinWeights = new();

    private Vector3 smoothedVelocity;

    void Start()
    {
        if (puppetMaster == null) puppetMaster = GetComponent<PuppetMaster>();
        if (characterController == null) characterController = GetComponent<CharacterController>();

        for (int i = 0; i < puppetMaster.muscles.Length; i++)
        {
            if (System.Array.Exists(targetMuscleNames, name => puppetMaster.muscles[i].name == name))
            {
                targetIndices.Add(i);
                originalPinWeights[i] = puppetMaster.muscles[i].props.pinWeight;
            }
        }

        if (targetIndices.Count == 0)
        {
            Debug.LogWarning("No matching muscles found for tilt.");
        }
    }

    void FixedUpdate()
    {
        if (puppetMaster == null || targetIndices.Count == 0) return;
        if (GameManager.instance.finished)
        {
            foreach (int i in targetIndices)
            {
                float newPin = Mathf.Clamp01(originalPinWeights[i] - drunkLevel * maxPinWeightOffset);
                puppetMaster.muscles[i].props.pinWeight = 1;
            }

            return;
        }

    // 1. Ottieni la direzione opposta al movimento
        Vector3 velocity = characterController.velocity;
        smoothedVelocity = Vector3.Lerp(smoothedVelocity, velocity, Time.deltaTime * tiltSmoothing);

        Vector3 tiltDir = -smoothedVelocity.normalized;

        if (tiltDir == Vector3.zero) return;

        // 2. Applica forza ai muscoli selezionati
        foreach (int i in targetIndices)
        {
            Vector3 force = tiltDir * (drunkLevel * maxTiltForce);
            puppetMaster.muscles[i].rigidbody.AddForce(force, ForceMode.Force);

            float newPin = Mathf.Clamp01(originalPinWeights[i] - drunkLevel * maxPinWeightOffset);
            puppetMaster.muscles[i].props.pinWeight = newPin;
        }
    }

    void OnDisable()
    {
        if (puppetMaster == null) return;

        foreach (int i in targetIndices)
        {
            if (puppetMaster.muscles[i] != null)
                puppetMaster.muscles[i].props.pinWeight = originalPinWeights[i];
        }
    }
}
