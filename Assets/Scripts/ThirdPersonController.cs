using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using RootMotion.Dynamics;

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
    [SerializeField] private PuppetMaster puppetMaster;

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
    
    [Header("Sounds")]
    [SerializeField] private AudioClip LandingAudioClip;
    [SerializeField] private AudioClip[] FootstepAudioClips;
    [Range(0, 1)] [SerializeField] private float FootstepAudioVolume = 0.5f;

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

        if (IsMoving)
        {
            float targetSpeed = isRunning ? runSpeed : moveSpeed;
            float speedParam = inputMagnitude * (targetSpeed / runSpeed);
            animator.SetFloat(InputMovement, speedParam, 0.1f, Time.deltaTime);
        }
    }

    void OnAnimatorMove()
    {
        if (!animator) return;
        if (animator.applyRootMotion)
        {
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
    }

    public void LaunchPuppet(Vector3 direction, float force)
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
        StartCoroutine(DieAfterJumping());
    }

    private IEnumerator DieAfterJumping()
    {
        yield return new WaitForSeconds(0.6f);
        behaviourPuppet.SetState(BehaviourPuppet.State.Unpinned);
        Vector3 diveDir = transform.forward + Vector3.up * 0.5f;
        LaunchPuppet(diveDir, launchForce);
    }

    // 🎮 New Input System handlers (connect via PlayerInput)
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
    
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (!(animationEvent.animatorClipInfo.weight > 0.5f)) return;
        if (FootstepAudioClips.Length <= 0) return;
        
        var index = Random.Range(0, FootstepAudioClips.Length);
        SoundFXManager.instance.PlaySoundFxClip(FootstepAudioClips[index], transform, FootstepAudioVolume);
    }
}
