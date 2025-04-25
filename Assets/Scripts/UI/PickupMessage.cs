using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PickupMessage : MonoBehaviour
{
    public static PickupMessage instance;
    
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI pickupName;
    [SerializeField] private TextMeshProUGUI pickupDescription;
    [SerializeField] float displayDuration = 0.8f;
    [SerializeField] float fadeDuration = 0.5f;
    
    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (instance == null) instance = this;
        canvasGroup = popupPanel.GetComponent<CanvasGroup>();
        popupPanel.SetActive(false);
    }

    public void Show(string itemName, string itemDescription)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        
        pickupName.text = itemName;
        pickupDescription.text = itemDescription;
        popupPanel.SetActive(true);
        canvasGroup.alpha = 1f;
        
        CancelInvoke(nameof(StartFadeOut));
        Invoke(nameof(StartFadeOut), displayDuration);
    }

    private void StartFadeOut()
    {
        fadeCoroutine = StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float startAlpha = 1f;
        float endAlpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        popupPanel.SetActive(false);
    }
}
