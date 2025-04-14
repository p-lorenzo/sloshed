using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    public Transform cameraTransform;
    public float rotationSpeed = 10f;
    public float moveSpeed = 2f;
    public float runSpeed = 4f;
    public float gravity = -9.81f;
    public float groundedCheckDistance = 0.1f;

    private Vector3 verticalVelocity = Vector3.zero;

    private Animator animator;
    private CharacterController controller;
    private bool IsMoving = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 rawInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        float inputMagnitude = Mathf.Clamp01(rawInput.magnitude);

        Vector3 moveDirection = Vector3.zero;
        if (inputMagnitude > 0.01f)
        {
            animator.SetBool("IsMoving", true);
            IsMoving = true;
            // Direzione relativa alla camera
            moveDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * rawInput.normalized;

            // Rotazione verso la direzione di movimento
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("IsMoving", false);
            IsMoving = false;
        }
        if (IsMoving)
        {
            float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;
            float speedParam = inputMagnitude * (targetSpeed / runSpeed);
            animator.SetFloat("InputMovement", speedParam, 0.1f, Time.deltaTime);
        }
        
    }
    
    void OnAnimatorMove()
    {
        if (animator.applyRootMotion)
        {
            Vector3 rootMotion = animator.deltaPosition;
            rootMotion.y = 0f; // Root Motion non controlla la Y

            // Gravità
            if (controller.isGrounded)
            {
                verticalVelocity.y = -1f; // Assicura che il controller resti incollato al terreno
            }
            else
            {
                verticalVelocity.y += gravity * Time.deltaTime;
            }

            // Applica Root Motion + Gravità
            Vector3 finalMotion = rootMotion + verticalVelocity * Time.deltaTime;
            controller.Move(finalMotion);
        }
    }
}