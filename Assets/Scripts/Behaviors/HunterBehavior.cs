using System;
using System.Collections;
using RootMotion.Dynamics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using UnityEngine.Serialization;
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
    private bool hasHitPlayer;
    
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private float dissolveDuration;
    
    [Header("Sounds")]
    [SerializeField] private AudioClip[] footstepAudioClips; 
    [Range(0, 1)] [SerializeField] private float footstepAudioVolume = 0.5f;
    [SerializeField] private AudioClip playerHitSound;
    [Range(0, 1)] [SerializeField] private float playerHitVolume = 0.5f;
    [SerializeField] private AudioClip hunterDeathSound;
    [Range(0, 1)] [SerializeField] private float hunterDeathVolume = 0.5f;
    
    private IObjectPool<HunterBehavior> hunterPool;

    public void SetPool(IObjectPool<HunterBehavior> pool)
    {
        hunterPool = pool;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerController").transform;
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
        agent = GetComponent<NavMeshAgent>();
        thirdPersonController = FindFirstObjectByType<ThirdPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("EvilBanisher")) return;
        agent.isStopped = true;
        dissolveDuration = 0.5f;
        StartCoroutine(Dissolve());
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        if (hasHitPlayer) return;
        SoundFXManager.instance.PlaySoundFxClip(playerHitSound, transform, playerHitVolume);
        StartCoroutine(Dissolve());
        //thirdPersonController.puppetMaster.state = PuppetMaster.State.Dead;
    }

    private IEnumerator Dissolve()
    {
        hasHitPlayer = true;
        animator.SetBool(IsPunching, true);
        
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
        
        deathParticles.Play();
        
        while (elapsed < dissolveDuration)
        {
            dissolveValue = Mathf.Lerp(0f, 1f, elapsed / dissolveDuration);
            mat.SetFloat("_Dissolve", dissolveValue);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mat.SetFloat("_Dissolve", 1f);
        mat.SetFloat("_Dissolve", 0f);
        SoundFXManager.instance.PlaySoundFxClip(hunterDeathSound, transform, hunterDeathVolume);
        
        thirdPersonController.puppetMaster.state = PuppetMaster.State.Alive;
        hasHitPlayer = false;
        hunterPool.Release(this);
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
        if (footstepAudioClips.Length <= 0) return;
        
        var index = Random.Range(0, footstepAudioClips.Length);
        SoundFXManager.instance.PlaySoundFxClip(footstepAudioClips[index], transform, footstepAudioVolume);
    }
}
