using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerFinishTracker _playerFinishTracker;

    private void Start()
    {
        _playerFinishTracker = FindFirstObjectByType<PlayerFinishTracker>();
    }

    public void Fallen()
    {
        if (_playerFinishTracker != null && _playerFinishTracker.IsOnBed())
        {
            Debug.Log("Fallen On Bed!");
        }
        else
        {
            Debug.Log("Fallen not on Bed!");
        }
    }
}
