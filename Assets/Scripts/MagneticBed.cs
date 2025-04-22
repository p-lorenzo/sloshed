using UnityEngine;

public class MagneticBed : MonoBehaviour
{
    [SerializeField] private float attractionForce = 3f;
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            Vector3 directionToCenter = (transform.position - other.transform.position).normalized;
            rb.AddForce(directionToCenter * attractionForce, ForceMode.Acceleration);
        }
    }
}
