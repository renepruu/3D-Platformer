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
    public float lookUpLimit = 45f;    // smaller = can't look too far up
    public float lookDownLimit = 80f; // How far up/down you can look

    private float xRotation = 0f; // Vertical camera rotation
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Camera playerCamera; // Automatically finds the child camera

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        // Preserve the camera’s starting angle to prevent snapping
        xRotation = playerCamera.transform.localEulerAngles.x;

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("✅ PlayerMovement initialized with smooth look and preserved camera angle");
    }

    void Update()
    {
        // --- Mouse Look ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera vertically (pitch)
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

        // --- Jump ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // --- Gravity ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
