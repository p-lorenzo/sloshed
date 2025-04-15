using UnityEngine;
using System.Collections.Generic;

public class PlayerFinishTracker : MonoBehaviour
{
    private readonly HashSet<Collider> activeFinishTriggers = new();

    /// <summary>
    /// Ritorna true se il player sta toccando almeno un trigger con tag "Finish"
    /// </summary>
    public bool IsOnBed()
    {
        return activeFinishTriggers.Count > 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            activeFinishTriggers.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            activeFinishTriggers.Remove(other);
        }
    }
}