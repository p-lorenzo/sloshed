using UnityEngine;

public class MobileDisabler : MonoBehaviour
{
    public bool isMobile()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer && Application.isMobilePlatform;
    }
    void Start()
    {
        gameObject.SetActive(!isMobile());
    }
}
