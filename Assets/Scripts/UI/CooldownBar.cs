using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CooldownBar : MonoBehaviour
{
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private float fadeDuration = 0.5f;
    
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image barLeft;
    [SerializeField] private Image barRight;

    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;

    private bool cooldownStarted;
    void Start()
    {
      if (thirdPersonController == null)
          thirdPersonController = GameObject.FindWithTag("PlayerController").GetComponent<ThirdPersonController>();
      if (canvasGroup == null)
          canvasGroup = GetComponent<CanvasGroup>();
    }
    
    void Update()
    {
        if (thirdPersonController.elapsedTime == 0 && cooldownStarted) HideBar();
        
        if (!thirdPersonController.itemIsOnCooldown) return;
        if (!cooldownStarted) ShowBar();
        
        var fillAmount = thirdPersonController.elapsedTime / thirdPersonController.itemUseCooldown;
        
        barLeft.fillAmount = fillAmount;
        barRight.fillAmount = fillAmount;
    }

    private void HideBar()
    {
        cooldownStarted = false;
        barLeft.color = endColor;
        barRight.color = endColor;
        StopAllCoroutines();
        StartCoroutine(FadeCanvasGroup(1f, 0f, fadeDuration));
    }
    
    private void ShowBar()
    {
        cooldownStarted = true;
        barLeft.color = startColor;
        barRight.color = startColor;
        StopAllCoroutines();
        StartCoroutine(FadeCanvasGroup(0f, 1f, fadeDuration * 2));
    }
    
    private IEnumerator FadeCanvasGroup(float from, float to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
    
}
