using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public float jumpHeight = 5f;
    public float gravity = -9.81f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Mouse Look")]
    public float mouseSensitivity = 70f;

    [Header("Vertical Look Limits")]
    public float lookUpLimit = 45f;
    public float lookDownLimit = 80f;

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
        // Mouse Look
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

        // Movement input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Apply movement + push
        controller.Move((move * speed + externalPush) * Time.deltaTime);

        // Fade the push force
        externalPush = Vector3.Lerp(externalPush, Vector3.zero, 5f * Time.deltaTime);

        // Animation: Walking
        bool isWalking = (x != 0 || z != 0);
        animator.SetBool("isWalking", isWalking);

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Jump/Fall animations
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
