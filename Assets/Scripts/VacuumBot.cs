using System;
using RootMotion.Dynamics;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class VacuumBot : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform player;
    [SerializeField] private float randomAngleRange = 60f;
    
    private Collider _lastCollider;
    private Rigidbody rb;
    
    private Vector3 lastDesiredDirection = Vector3.zero;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.up * (moveSpeed * Time.fixedDeltaTime));
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
        float angleToPlayer = Vector3.SignedAngle(transform.up, toPlayer, Vector3.forward); // Z-axis for 2D
        float randomOffset = Random.Range(-randomAngleRange, randomAngleRange);
        float finalAngle = angleToPlayer + randomOffset;

        transform.Rotate(0f, 0f, finalAngle);

        // Store direction after rotation for Gizmo
        lastDesiredDirection = Quaternion.Euler(0f, 0f, finalAngle) * transform.up;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Bot's current forward direction
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 1.5f);

        // Last calculated "desired" direction
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + lastDesiredDirection * 1.5f);
    }
}