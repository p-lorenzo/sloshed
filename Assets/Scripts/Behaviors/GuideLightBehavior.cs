using UnityEngine;
using UnityEngine.AI;

public class GuideLightBehavior : MonoBehaviour
{
    [SerializeField] private GameObject guideLightTrail;
    [SerializeField] private ParticleSystem guideLightDeathParticle;
    [SerializeField] private AudioClip guideLightDeathSound;
    [Range(0,1)] [SerializeField] private float guideLightDeathVolume = 0.5f;
    
    private Transform target;
    private NavMeshAgent agent;

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Finish").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!target) return;
        
        agent.SetDestination(target.position);
        
        if (agent.remainingDistance == 0) return;
        if (!(agent.remainingDistance <= agent.stoppingDistance)) return;
        
        guideLightDeathParticle.transform.parent = null;
        guideLightDeathParticle.Play();
        
        guideLightTrail.transform.parent = null;
        
        SoundFXManager.instance.PlaySoundFxClip(guideLightDeathSound, gameObject.transform, guideLightDeathVolume);
        Destroy(gameObject);
        
        var deathParticleDuration = guideLightDeathParticle.main.duration + guideLightDeathParticle.main.startLifetime.constantMax;
        Destroy(guideLightDeathParticle.gameObject, deathParticleDuration);
    }
}
