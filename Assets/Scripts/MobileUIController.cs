using UnityEngine;
using UnityEngine.EventSystems;

public class MobileUIController : MonoBehaviour
{
    void Start()
    {
        if (IsMobileBrowser())
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    bool IsMobileBrowser()
    {
#if UNITY_EDITOR
        return false;
#elif UNITY_WEBGL
        return Input.touchSupported;
#else
        return Application.isMobilePlatform;
#endif
    }
}