using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementStrafe : MonoBehaviour
{
    [Header("Movement Settings (Strafe Version)")]
    public float strafeSpeed = 3f;
    public float strafeJumpHeight = 5f;
    public float strafeGravity = -9.81f;

    [Header("Ground Check")]
    public Transform strafeGroundCheck;
    public float strafeGroundDistance = 0.4f;
    public LayerMask strafeGroundMask;

    [Header("Mouse Look")]
    public float strafeMouseSensitivity = 70f;

    [Header("Vertical Look Limits")]
    public float strafeLookUpLimit = 30f;
    public float strafeLookDownLimit = 30f;

    [Header("Animation Settings")]
    public float strafeFallThreshold = -0.1f;

    private float strafeXRotation = 0f;
    private CharacterController strafeController;
    private Vector3 strafeVelocity;
    private bool strafeIsGrounded;
    private Camera strafeCamera;

    private Animator strafeAnimator;

    private Vector3 strafeExternalPush = Vector3.zero;

    void Start()
    {
        strafeController = GetComponent<CharacterController>();
        strafeCamera = GetComponentInChildren<Camera>();
        strafeAnimator = GetComponentInChildren<Animator>();

        strafeXRotation = strafeCamera.transform.localEulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- Mouse Look ---
        float mouseX = Input.GetAxis("Mouse X") * strafeMouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * strafeMouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        strafeXRotation -= mouseY;
        strafeXRotation = Mathf.Clamp(strafeXRotation, -strafeLookDownLimit, strafeLookUpLimit);
        strafeCamera.transform.localRotation = Quaternion.Euler(strafeXRotation, 0f, 0f);

        // Ground Check
        strafeIsGrounded = Physics.CheckSphere(strafeGroundCheck.position, strafeGroundDistance, strafeGroundMask);

        if (strafeIsGrounded && strafeVelocity.y < 0 && strafeExternalPush.magnitude < 0.05f)
            strafeVelocity.y = -2f;

        // Movement Input (STRAFE + FORWARD/BACK)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        strafeController.Move((move * strafeSpeed + strafeExternalPush) * Time.deltaTime);

        strafeExternalPush = Vector3.Lerp(strafeExternalPush, Vector3.zero, 5f * Time.deltaTime);

        // Animation
        bool isWalking = move.magnitude > 0.01f;
        if (strafeAnimator != null)
            strafeAnimator.SetBool("isWalking", isWalking);

        // Jump
        if (Input.GetButtonDown("Jump") && strafeIsGrounded)
            strafeVelocity.y = Mathf.Sqrt(strafeJumpHeight * -2f * strafeGravity);

        bool isJumpingAnim = !strafeIsGrounded && strafeVelocity.y > 0.1f;
        bool isFallingAnim = !strafeIsGrounded && strafeVelocity.y < strafeFallThreshold;

        if (strafeAnimator != null)
        {
            strafeAnimator.SetBool("isJumping", isJumpingAnim);
            strafeAnimator.SetBool("isFalling", isFallingAnim);
        }

        // Gravity
        strafeVelocity.y += strafeGravity * Time.deltaTime;
        strafeController.Move(strafeVelocity * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.attachedRigidbody == null)
            return;

        Vector3 push = hit.moveDirection;
        push.y = 0;

        strafeExternalPush += push * 2f;
    }
}
