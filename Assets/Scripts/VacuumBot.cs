using System;
using RootMotion.Dynamics;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class VacuumBot : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private BehaviourPuppet behaviourPuppet;
    private Collider _lastCollider;
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
        behaviourPuppet = FindAnyObjectByType<BehaviourPuppet>();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.up * (moveSpeed * Time.fixedDeltaTime));
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other == _lastCollider) return;
        _lastCollider = other;
        Debug.Log("OnTriggerEnter: " + other.name);
        if (!other.CompareTag("Player"))
        {
            Debug.Log("Obstacle detected!");
            float randomTurn = Random.Range(-90f, 90f);
            transform.Rotate(0f, 0f, randomTurn);  
        }
        else
        {
            behaviourPuppet.SetState(BehaviourPuppet.State.Unpinned);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other == _lastCollider) return;
        _lastCollider = other;
        Debug.Log("OnTriggerStay: " + other.name);
        if (!other.CompareTag("Player"))
        {
            Debug.Log("Obstacle detected!");
            float randomTurn = Random.Range(-90f, 90f);
            transform.Rotate(0f, 0f, randomTurn);  
        }
        else
        {
            behaviourPuppet.SetState(BehaviourPuppet.State.Unpinned);
        }
    }
}