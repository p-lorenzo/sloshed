using System;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class DrunkEffectController : MonoBehaviour
{
    public static DrunkEffectController instance;
    
    [Header("Camera Noise Settings")] 
    [SerializeField] private float cameraNoiseDrunknessMultiplier = 1.5f;

    [Header("References")] 
    private CinemachineBasicMultiChannelPerlin cameraNoise;
    private CharacterController controller;
    private ThirdPersonController thirdPersonController;
    private TextMeshProUGUI drunknessMeter;
    private GameManager gameManager;
    private Volume globalVolume;
    private DrunkPuppetTilt playerTilt;

    [Header("Audio Settings")] 
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float minCutoffFreq = 100f;
    [SerializeField] private float maxCutoffFreq = 22000f;
    [SerializeField] private AudioSource ringingSound;
    [SerializeField] private float maxRingingSound = 0.01f;
    [SerializeField] private float minRingingSound = 0f;

    [Header("Drunkness Settings")] 
    [SerializeField] private Material drunkMat;
    [Range(0f, .5f)] [SerializeField] private float drunkLevel = 0f;
    [SerializeField] private float drunkGainRate = 0.03f;
    [SerializeField] private float drunkDecayRate = 0.1f;

    [Header("Player Drift Settings")] 
    [SerializeField] private float driftMagnitude = 1.2f;
    private Vector3 targetDrift;
    private Vector3 currentDrift;
    private float driftCooldown = 3f;
    private float driftTimer = 0f;
    
    [Header("LightBloom Settings")] 
    private Bloom _bloom;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        drunkLevel = 0f;
        RebindReferences();
    }    
    
    private void RebindReferences()
    {
        // Rebind References
        if (cameraNoise == null)
            cameraNoise = FindAnyObjectByType<CinemachineBasicMultiChannelPerlin>();
        if (controller == null)
            controller = FindAnyObjectByType<CharacterController>();
        if (thirdPersonController == null)
            thirdPersonController = FindAnyObjectByType<ThirdPersonController>();
        if (drunknessMeter == null)
        {
            var meterObj = GameObject.Find("DrunkMeter");
            if (meterObj != null)
                drunknessMeter = meterObj.GetComponent<TextMeshProUGUI>();
            else
                Debug.LogWarning("DrunkMeter UI element not found!");
        }
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();
        if (globalVolume == null)
            globalVolume = FindAnyObjectByType<Volume>();
        if (playerTilt == null)
            playerTilt = FindAnyObjectByType<DrunkPuppetTilt>();

        // Recupera il Bloom se possibile
        if (globalVolume != null && globalVolume.profile != null)
            globalVolume.profile.TryGet(out _bloom);

        if (_bloom != null)
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
        drunkMat.SetFloat("_ChromaticAberration", drunkLevel * 0.02f);

        audioMixer.SetFloat("_CutoffFreq", DrunknessToAudioValue(maxCutoffFreq, minCutoffFreq));
        ringingSound.volume = DrunknessToAudioValue(minRingingSound, maxRingingSound);
        /*_bloom.threshold.value = .5f - drunkLevel;
        _bloom.intensity.value = drunkLevel * 50;*/
    }

    private void OnDestroy()
    {
        drunkMat.SetFloat("_Amplitude", 0f);
        drunkMat.SetFloat("_GhostStrength", 1f);
        drunkMat.SetFloat("_WaveSpeed", 0f);
        drunkMat.SetFloat("_ChromaticAberration", 0f);
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

    public void DrinkWater()
    {
        drunkLevel = Mathf.Clamp(drunkLevel - 0.025f, 0f, .5f);
        drunkGainRate *= 0.9f;
    }

    public void RestartLevel()
    {
        drunkGainRate = 0.03f;
        drunkDecayRate = 0.1f;
    }
}