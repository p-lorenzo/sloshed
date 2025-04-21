using System;
using RootMotion.Dynamics;
using UnityEngine;

public class ForceFall : MonoBehaviour
{
    [SerializeField] private BehaviourPuppet behaviourPuppet;

    private void Start()
    {
        behaviourPuppet = FindFirstObjectByType<BehaviourPuppet>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        behaviourPuppet.SetState(BehaviourPuppet.State.Unpinned);
    }
}
