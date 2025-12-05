using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public float jumpHeight = 5f;
    public float gravity = -9.81f;
    public float turnSpeed = 200f; // Rotation smoothing speed

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Mouse Look")]
    public float mouseSensitivity = 70f;

    [Header("Vertical Look Limits")]
    public float lookUpLimit = 30f;
    public float lookDownLimit = 30f;

    [Header("Animation Settings")]
    public float fallThreshold = -0.1f;

    private float xRotation = 0f;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Camera playerCamera;

    private Animator animator;

    // Push from moving obstacles
    private Vector3 externalPush = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        animator = GetComponentInChildren<Animator>();

        if (PortalSceneManager.Instance != null)
        {
            PortalSceneManager.Instance.ApplySavedTransformToPlayer(transform);
        }

        xRotation = playerCamera.transform.localEulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- Mouse Look ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -lookDownLimit, lookUpLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // 🔥 FIX: Prevent CC from forcing downward velocity during push
        if (isGrounded && velocity.y < 0 && externalPush.magnitude < 0.05f)
            velocity.y = -2f;

        // --- Movement Input ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Keyboard horizontal input rotates the player (A/D turns), forward/back (W/S) moves the player.
        if (Mathf.Abs(x) > 0.01f)
        {
            transform.Rotate(Vector3.up * x * turnSpeed * Time.deltaTime);
        }

        // Only move forward/back relative to player facing
        Vector3 move = transform.forward * z;
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        // Move the player (horizontal movement). Vertical motion handled by `velocity` + gravity below.
        controller.Move(move * speed * Time.deltaTime);

        // --- ANIMATION: Walking ---
        // Only consider forward/back input (W/S) as walking. A/D rotates the player and should not trigger walk.
        bool isWalking = Mathf.Abs(z) > 0.01f;
        if (animator != null)
            animator.SetBool("isWalking", isWalking);

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        bool isJumpingAnim = !isGrounded && velocity.y > 0.1f;
        bool isFallingAnim = !isGrounded && velocity.y < fallThreshold;

        if (animator != null)
        {
            animator.SetBool("isJumping", isJumpingAnim);
            animator.SetBool("isFalling", isFallingAnim);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Push from moving obstacles
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.attachedRigidbody == null)
            return;

        Vector3 push = hit.moveDirection;
        push.y = 0;

        externalPush += push * 2f; // push strength
    }
}
