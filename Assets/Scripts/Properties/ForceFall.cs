using System;
using UnityEngine;

public class ForceFall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        GameManager.instance.FinishRound();
    }
}
