using System.Collections;
using RootMotion;
using UnityEngine;
using UnityEngine.InputSystem;
using RootMotion.Dynamics;
using UnityEngine.Serialization;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    private static readonly int Diving = Animator.StringToHash("Diving");
    private static readonly int Moving = Animator.StringToHash("IsMoving");
    private static readonly int InputMovement = Animator.StringToHash("InputMovement");

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Puppet stuff")]
    [SerializeField] private BehaviourPuppet behaviourPuppet;
    [SerializeField] public PuppetMaster puppetMaster;

    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float launchForce = 5f;

    public Vector3 InputOffset = Vector3.zero;

    private Vector3 verticalVelocity = Vector3.zero;
    private Animator animator;
    private CharacterController controller;
    private bool IsMoving = false;
    private bool hasJumped = false;

    private Vector2 moveInput = Vector2.zero;
    private bool isRunning = false;
    private int speedDemonCount;
    private bool stickyFeet;
    private float moveSpeedModifier = 1f;
    
    [Header("Sounds")]
    [SerializeField] private AudioClip landingAudioClip;
    [SerializeField] private AudioClip[] footstepAudioClips;
    [Range(0, 1)] [SerializeField] private float footstepAudioVolume = 0.5f;
    [SerializeField] private AudioClip[] stickyFeetAudioClips;
    [Range(0,1)] [SerializeField] private float stickyFeetAudioVolume = 0.2f;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 rawInput = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 offsettedInput = rawInput;
        float inputMagnitude = Mathf.Clamp01(offsettedInput.magnitude);

        Vector3 moveDirection = Vector3.zero;
        if (inputMagnitude > 0.01f)
        {
            offsettedInput += InputOffset;
            animator.SetBool(Moving, true);
            IsMoving = true;

            // Direzione relativa alla camera
            moveDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * offsettedInput.normalized;

            // Rotazione verso la direzione di movimento
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool(Moving, false);
            IsMoving = false;
        }

        if (!IsMoving) return;
        
        var targetSpeed = isRunning ? runSpeed : moveSpeed;
        var speedParam = inputMagnitude * (targetSpeed / runSpeed);
        
        animator.SetFloat(InputMovement, speedParam * moveSpeedModifier, 0.1f, Time.deltaTime);
    }

    public void CheckPowerups()
    {
        if (PowerupManager.instance.HasAtLeastOnePowerUpOfType(PowerupManager.PowerupType.StickyFeet)) SetStickyFeet();
        SetSpeedDemon();
        SetMoveSpeedModifier();
    }

    public void SetMoveSpeedModifier()
    {
        moveSpeedModifier *= (1f + .2f * speedDemonCount);
        if (stickyFeet) moveSpeedModifier *= .8f;
    }

    public void SetSpeedDemon()
    {
        speedDemonCount = PowerupManager.instance.HowManyPowerupsOfType(PowerupManager.PowerupType.SpeedDemon);
    }

    public void SetStickyFeet()
    {
        stickyFeet = true;
        behaviourPuppet.collisionLayers = 0;
    }

    private void OnAnimatorMove()
    {
        if (!animator) return;
        if (!animator.applyRootMotion) return;
        
        Vector3 rootMotion = animator.deltaPosition;
        
        rootMotion.y = 0f;

        if (controller.isGrounded)
        {
            verticalVelocity.y = -1f;
        }
        else
        {
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        Vector3 finalMotion = rootMotion + verticalVelocity * Time.deltaTime;
        controller.Move(finalMotion);
    }

    private void LaunchPuppet(Vector3 direction, float force)
    {
        foreach (var muscle in puppetMaster.muscles)
        {
            muscle.rigidbody.AddForce(direction * force, ForceMode.Impulse);
        }
    }

    public void Dive()
    {
        if (hasJumped) return;
        hasJumped = true;
        animator.SetBool(Diving, true);
        StartCoroutine(UnpinAfterJumping());
    }
    
    private IEnumerator UnpinAfterJumping()
    {
        yield return new WaitForSeconds(0.6f);
        behaviourPuppet.SetState(BehaviourPuppet.State.Unpinned);
        Vector3 diveDir = transform.forward + Vector3.up * 0.5f;
        LaunchPuppet(diveDir, launchForce);
    }

    public void UnDive()
    {
        if (!hasJumped) return;
        hasJumped = false;
        animator.SetBool(Diving, false);
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isRunning = true;
        }
        else if (context.canceled)
        {
            isRunning = false;
        }
    }
    
    public void StartSprintUI()
    {
        isRunning = true;
    }

    public void StopSprintUI()
    {
        isRunning = false;
    }

    public void OnDive(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Dive();
        }
    }

    public void OnHolyWaterUse(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.instance.UseHolyWater();
        }
    }
    
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (!(animationEvent.animatorClipInfo.weight > 0.5f)) return;
        if (footstepAudioClips.Length <= 0) return;
        
        var index = Random.Range(0, footstepAudioClips.Length);
        SoundFXManager.instance.PlaySoundFxClip(footstepAudioClips[index], transform, footstepAudioVolume);

        if (!PowerupManager.instance.HasAtLeastOnePowerUpOfType(PowerupManager.PowerupType.StickyFeet)) return;
        
        var index2 = Random.Range(0, stickyFeetAudioClips.Length);
        SoundFXManager.instance.PlaySoundFxClip(stickyFeetAudioClips[index2], transform, stickyFeetAudioVolume);
    }
}
