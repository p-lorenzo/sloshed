using System;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private bool cursorLocked = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !cursorLocked && GameManager.instance.started && !GameManager.instance.finished)
        {
            LockCursor();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && cursorLocked)
        {
            UnlockCursor();
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorLocked = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorLocked = false;
    }
}