using UnityEngine;
using System.Collections.Generic;
using RootMotion.Dynamics;

public class PlayerFinishTracker : MonoBehaviour
{
    private readonly HashSet<Collider> activeFinishTriggers = new();
    [SerializeField] private BehaviourPuppet behaviourPuppet;
    [SerializeField] private PuppetMaster puppetMaster;
    [SerializeField] private float launchForce = 5f;
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
            behaviourPuppet.SetState(BehaviourPuppet.State.Unpinned);
            DiveOntoBed();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            activeFinishTriggers.Remove(other);
        }
    }
    
    public void LaunchPuppet(Vector3 direction, float force)
    {
        foreach (var muscle in puppetMaster.muscles)
        {
            muscle.rigidbody.AddForce(direction * force, ForceMode.Impulse);
        }
    }
    
    public void DiveOntoBed()
    {
        Vector3 diveDir = transform.forward + Vector3.up * 0.5f;
        LaunchPuppet(diveDir.normalized, launchForce);
    }
}