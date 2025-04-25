using UnityEngine;

public class MobileUIController : MonoBehaviour
{
    public bool isMobile()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer && Application.isMobilePlatform;
    }
    void Start()
    {
        gameObject.SetActive(isMobile());
    }
}