using UnityEngine;

public class FlashlightManager : MonoBehaviour
{
    [SerializeField] private GameObject flashlight;
    void Start()
    {
        flashlight.SetActive(PowerupManager.instance.HasAtLeastOnePowerUpOfType(PowerupManager.PowerupType.Flashlight));
    }

    public void ActivateFlashlight()
    {
        flashlight.SetActive(true);
    }
}
