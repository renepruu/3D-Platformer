using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class FootstepSystem : MonoBehaviour
{
    [Header("Settings")]
    public float stepInterval = 0.5f;
    [Range(0f, 1f)] public float volume = 1f;

    [Header("Audio Clips")]
    public AudioClip footstepSound1;
    public AudioClip footstepSound2;
    public AudioClip jumpSound;

    private CharacterController characterController;
    private AudioSource audioSource;
    private float stepTimer = 0f;
    private bool useFirstSound = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        HandleFootsteps();
        HandleJump();
    }

    void HandleFootsteps()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        bool isInputActive = Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputY) > 0.1f;
        bool isGrounded = characterController.isGrounded;

        if (isGrounded && isInputActive)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayAlternatingFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0.1f;
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && characterController.isGrounded)
        {
            if (jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound, volume);
            }
        }
    }

    void PlayAlternatingFootstep()
    {
        audioSource.volume = volume;

        if (useFirstSound)
        {
            if (footstepSound1 != null) audioSource.PlayOneShot(footstepSound1);
        }
        else
        {
            if (footstepSound2 != null) audioSource.PlayOneShot(footstepSound2);
        }

        useFirstSound = !useFirstSound;
    }
}