using System;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    
    public bool isPaused = false;
    [SerializeField] private GameObject hud;
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject rawImage;
    [SerializeField] private RenderTexture pixelatedTexture;
    
    public void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer && Application.isMobilePlatform)
        {
            RemovePixelatedEffect();
        }
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
    }

    public void EnablePixelatedEffect()
    {
        rawImage.SetActive(true);
        camera.targetTexture = pixelatedTexture;
    }

    public void TogglePixelatedEffect(bool state)
    {
        if (state) EnablePixelatedEffect();
        else RemovePixelatedEffect();
    }
}
