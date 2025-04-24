using System;
using RootMotion.Dynamics;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class VacuumBot : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform player;
    [SerializeField] private float randomAngleRange = 15f;
    
    private Collider _lastCollider;
    private Rigidbody rb;
    
    private Vector3 lastDesiredDirection = Vector3.zero;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (player == null)
            player = GameObject.FindWithTag("PlayerController")?.transform;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.forward * (moveSpeed * Time.fixedDeltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other == _lastCollider) return;
        _lastCollider = other;
        SteerTowardPlayer();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other == _lastCollider) return;
        _lastCollider = other;
        SteerTowardPlayer();
    }
    
    private void SteerTowardPlayer()
    {
        if (player == null) return;
        
        Vector3 toPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.SignedAngle(transform.forward, toPlayer, Vector3.up); // Z-axis for 2D
        float randomOffset = Random.Range(-randomAngleRange, randomAngleRange);
        float finalAngle = angleToPlayer + randomOffset;

        transform.Rotate(0f, finalAngle, 0f);

        // Store direction after rotation for Gizmo
        lastDesiredDirection = Quaternion.Euler(0f, finalAngle, 0f) * transform.forward;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Bot's current forward direction
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 1.5f);

        // Last calculated "desired" direction
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + lastDesiredDirection * 1.5f);
    }
}