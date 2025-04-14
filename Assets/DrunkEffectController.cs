using UnityEngine;

public class DrunkEffectController : MonoBehaviour
{
    public Material drunkMat;
    [Range(0f, 1f)] public float drunkLevel = 0.5f;

    void Update()
    {
        drunkMat.SetFloat("_Amplitude", drunkLevel * 0.03f);
        drunkMat.SetFloat("_GhostStrength", drunkLevel);
        drunkMat.SetFloat("_WaveSpeed", Mathf.Lerp(1f, 10f, drunkLevel));
    }
}