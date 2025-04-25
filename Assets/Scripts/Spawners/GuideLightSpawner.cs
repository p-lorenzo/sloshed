using System;
using UnityEngine;

public class GuideLightSpawner : MonoBehaviour
{
    public bool isPowerupActive = false;
    private float timerSpawner;
    [SerializeField] private GameObject guideLightPrefab;

    private void Start()
    {
        isPowerupActive = PowerupManager.instance.HasAtLeastOnePowerUpOfType(PowerupManager.PowerupType.GuideLight);
    }

    void Update()
    {
        if (!isPowerupActive) return;
        timerSpawner += Time.deltaTime;
        if (timerSpawner < 5f) return;
        timerSpawner = 0f;
        Instantiate(guideLightPrefab, transform.position, Quaternion.identity);
    }
}
