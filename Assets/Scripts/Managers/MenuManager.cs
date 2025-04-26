using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    
    public bool isPaused = false;
    [SerializeField] private GameObject hud;
    
    [Header("Stuff for pixelated setting")]
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject rawImage;
    [SerializeField] private RenderTexture pixelatedTexture;
    [SerializeField] private Toggle pixelatedToggle;
    private bool pixelated = true;

    [Header("Owned Powerups stuff")] [SerializeField]
    private GameObject powerupList;
    
    public void Awake()
    {
        if (instance == null) instance = this;
        pixelated = PlayerPrefs.GetInt("pixelated") == 1;
        pixelatedToggle.isOn = pixelated;
        if (!pixelated) RemovePixelatedEffect();
        gameObject.SetActive(false);
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (!GameManager.instance.started || GameManager.instance.finished) return;
        
        GameManager.instance.AddDepthOfField();
        isPaused = true;
        gameObject.SetActive(true);
        hud.SetActive(false);
        Time.timeScale = 0;
        UpdatePowerupList();
    }

    public void ResumeGame()
    {
        GameManager.instance.RemoveDepthOfField();
        CursorManager.instance.LockCursor();
        isPaused = false;
        gameObject.SetActive(false);
        hud.SetActive(true);
        Time.timeScale = 1;
    }

    public void RemovePixelatedEffect()
    {
        camera.targetTexture = null;
        rawImage.SetActive(false);
        pixelated = false;
        PlayerPrefs.SetInt("pixelated", pixelated ? 1 : 0);
    }

    public void EnablePixelatedEffect()
    {
        rawImage.SetActive(true);
        camera.targetTexture = pixelatedTexture;
        pixelated = true;
        PlayerPrefs.SetInt("pixelated", pixelated ? 1 : 0);
    }

    public void TogglePixelatedEffect(bool state)
    {
        if (state) EnablePixelatedEffect();
        else RemovePixelatedEffect();
    }

    private void UpdatePowerupList()
    {
        foreach (var powerup in PowerupManager.instance.GetOwnedPowerups())
        {
            powerupList.transform.Find(powerup.Key.ToString()).GetComponent<TextMeshProUGUI>().text = $"{powerup.Key}: {powerup.Value}";
        }
    }
}
