using UnityEngine;

public class GuideLightSpawner : MonoBehaviour
{
    private float timerSpawner;
    [SerializeField] private GameObject guideLightPrefab;
    
    void Update()
    {
        timerSpawner += Time.deltaTime;
        if (timerSpawner < 5f) return;
        timerSpawner = 0f;
        Instantiate(guideLightPrefab, transform.position, Quaternion.identity);
    }
}
