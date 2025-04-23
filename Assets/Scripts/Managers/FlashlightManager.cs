using UnityEngine;

public class FlashlightManager : MonoBehaviour
{
    [SerializeField] private GameObject flashlight;
    void Start()
    {
        flashlight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        flashlight.SetActive(DrunkEffectController.instance.flashlightActive);
    }
}
