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
    // threshold to consider player as falling (velocity.y less than this)
    public float fallThreshold = -0.1f;
    private float xRotation = 0f;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Camera playerCamera;

    // --- Animator reference ---
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        // find Animator in child
        animator = GetComponentInChildren<Animator>();

        // Apply saved position/rotation if we just portaled from another scene
        if (PortalSceneManager.Instance != null)
        {
            PortalSceneManager.Instance.ApplySavedTransformToPlayer(transform);
        }

        xRotation = playerCamera.transform.localEulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("PlayerMovement initialized with portal support.");
    }

    void Update()
    {
        // --- Mouse Look ---
        // Debug.Log("Animator found? " + animator);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -lookDownLimit, lookUpLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // --- Ground Check ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // --- Movement ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // --- ANIMATION: Walking ---
        bool isWalking = (x != 0 || z != 0);
        animator.SetBool("isWalking", isWalking);

        // --- ANIMATION: Jumping/Falling ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Determine animation states from vertical velocity and grounded state
        bool isJumpingAnim = !isGrounded && velocity.y > 0.1f;
        bool isFallingAnim = !isGrounded && velocity.y < fallThreshold;

        if (animator != null)
        {
            animator.SetBool("isJumping", isJumpingAnim);
            animator.SetBool("isFalling", isFallingAnim);
        }

        // --- Gravity ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- Portal hotkey ---
        if (Input.GetKeyDown(KeyCode.E)) // change key if you like
        {
            if (PortalSceneManager.Instance != null)
            {
                PortalSceneManager.Instance.SwitchWorld(transform);
            }
            else
            {
                Debug.LogWarning("No PortalSceneManager present in the scene.");
            }
        }
    }
}
