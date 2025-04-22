using System;
using UnityEngine;

public class ForceFall : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        GameManager.instance.Fallen();
    }
}
