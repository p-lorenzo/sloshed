using UnityEngine;

public class DisablePixelatedOnMobile : MonoBehaviour
{
    [SerializeField] private Camera camera;

    void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer && Application.isMobilePlatform)
        {
            camera.targetTexture = null;
            gameObject.SetActive(false);   
        }
    }
    
}
