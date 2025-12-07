using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement1 : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    
    [Header("Rotation Settings")]
    [SerializeField] private float bodyRotationSpeed = 10f;
    [SerializeField] private float maxBodyRotationAngle = 25f;
    [SerializeField] private float rotationResetSpeed = 5f;
    
    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerModel;
    [SerializeField] private Transform upperBody;
    
    private Vector3 velocity;
    private bool isGrounded;
    private float currentBodyRotation;
    private Vector3 lastMoveDirection;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (controller == null)
            Debug.LogError("PlayerMovement1: CharacterController not assigned and none found on the GameObject.");

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (playerModel == null && animator != null)
            playerModel = animator.transform;
    }

    void Update()
    {
        if (controller == null) return;

        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        HandleGravity();
        HandleBodyRotation();
        UpdateAnimator();
    }
    
    private void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded || 
                    Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
    
    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical);
        
        if (moveDirection.magnitude >= 0.1f)
        {
            moveDirection.Normalize();
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
            
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            playerModel.rotation = Quaternion.Slerp(
                playerModel.rotation, 
                targetRotation, 
                bodyRotationSpeed * Time.deltaTime
            );
            
            lastMoveDirection = moveDirection;
        }
    }
    
    private void HandleBodyRotation()
    {
        float horizontal = Input.GetAxis("Horizontal");
        
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            float targetRotation = -horizontal * maxBodyRotationAngle;
            
            currentBodyRotation = Mathf.Lerp(
                currentBodyRotation, 
                targetRotation, 
                bodyRotationSpeed * Time.deltaTime
            );
        }
        else
        {
            currentBodyRotation = Mathf.Lerp(
                currentBodyRotation, 
                0f, 
                rotationResetSpeed * Time.deltaTime
            );
        }
        
        if (upperBody != null)
        {
            upperBody.localRotation = Quaternion.Euler(0f, currentBodyRotation, 0f);
        }
        else
        {
            animator.SetFloat("BodyRotation", currentBodyRotation / maxBodyRotationAngle);
        }
    }
    
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            
            animator.SetTrigger("Jump");
            animator.SetBool("IsJumping", true);
        }
    }
    
    private void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        if (!isGrounded && velocity.y < 0 && animator.GetBool("IsJumping"))
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", true);
        }
    }
    
    private void UpdateAnimator()
    {
        Vector3 localVelocity = playerModel.InverseTransformDirection(controller.velocity);
        
        animator.SetFloat("ForwardSpeed", localVelocity.z);
        animator.SetFloat("StrafeSpeed", localVelocity.x);
        
        animator.SetFloat("VerticalVelocity", velocity.y);
        
        animator.SetBool("IsGrounded", isGrounded);

        if (isGrounded && animator.GetBool("IsFalling"))
        {
            animator.SetBool("IsFalling", false);
            animator.SetTrigger("Land");
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, groundCheckDistance);
    }
}