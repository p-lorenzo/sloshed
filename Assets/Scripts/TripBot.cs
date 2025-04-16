using System;
using RootMotion.Dynamics;
using UnityEngine;

public class TripBot : MonoBehaviour
{
    [SerializeField] private BehaviourPuppet behaviourPuppet;
    [SerializeField] private float legPinDecreaseRate = 0.01f;
    [SerializeField] private float minLegPinWeight = 0.0f;
    void Start()
    {
        behaviourPuppet = FindAnyObjectByType<BehaviourPuppet>();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (var m in behaviourPuppet.puppetMaster.muscles)
        {
            if (m.props.group == Muscle.Group.Leg)
            {
                m.props.pinWeight = Mathf.Max(minLegPinWeight, m.props.pinWeight - legPinDecreaseRate * Time.deltaTime);
            }
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        // Optional: reset leg pin weights when contact ends
        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (var m in behaviourPuppet.puppetMaster.muscles)
        {
            if (m.props.group == Muscle.Group.Leg)
            {
                m.props.pinWeight = 1f; 
            }
        }
    }
}
