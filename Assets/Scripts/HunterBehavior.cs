using System;
using System.Collections;
using RootMotion.Dynamics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class HunterBehavior : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int IsPunching = Animator.StringToHash("isPunching");
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private ThirdPersonController thirdPersonController;
    
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private float dissolveDuration;
    
    [Header("Sounds")]
    [SerializeField] private AudioClip[] FootstepAudioClips;
    [Range(0, 1)] [SerializeField] private float FootstepAudioVolume = 0.5f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
        agent = GetComponent<NavMeshAgent>();
        thirdPersonController = FindFirstObjectByType<ThirdPersonController>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        animator.SetBool(IsPunching, true);
        StartCoroutine(Dissolve());
        thirdPersonController.puppetMaster.state = PuppetMaster.State.Dead;
    }

    private IEnumerator Dissolve()
    {
        Transform meshTransform = transform.Find("casual_Male_K");
        if (!meshTransform)
        {
            Debug.LogWarning("Sub-object of Hunter 'casual_Male_K' not found.");
            yield break;
        }

        SkinnedMeshRenderer meshRenderer = meshTransform.GetComponent<SkinnedMeshRenderer>();
        if (!meshRenderer)
        {
            Debug.LogWarning("SkinnedMeshRenderer not found on Hunter's 'casual_Male_K'.");
            yield break;
        }

        Material mat = meshRenderer.material; // Creates a runtime instance
        float dissolveValue = 0f;
        float elapsed = 0f;

        deathParticles.transform.parent = null;
        deathParticles.Play();
        
        while (elapsed < dissolveDuration)
        {
            dissolveValue = Mathf.Lerp(0f, 1f, elapsed / dissolveDuration);
            mat.SetFloat("_Dissolve", dissolveValue);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mat.SetFloat("_Dissolve", 1f);
        Destroy(gameObject);
        
        thirdPersonController.puppetMaster.state = PuppetMaster.State.Alive;
        
        var totalDuration = deathParticles.main.duration + deathParticles.main.startLifetime.constantMax;
        Destroy(deathParticles.gameObject, totalDuration);
    }

    private void Update()
    {
        agent.SetDestination(player.position);
        var isMoving = agent.remainingDistance > agent.stoppingDistance;
        animator.SetBool(IsMoving, isMoving);
    }
    
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (!(animationEvent.animatorClipInfo.weight > 0.5f)) return;
        if (FootstepAudioClips.Length <= 0) return;
        
        var index = Random.Range(0, FootstepAudioClips.Length);
        SoundFXManager.instance.PlaySoundFxClip(FootstepAudioClips[index], transform, FootstepAudioVolume);
    }
}
