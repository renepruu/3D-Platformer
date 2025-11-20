using UnityEngine;

public class PlayerAnimationInput : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get player input (WASD / Arrow keys)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Determine if moving
        bool isMoving = (h != 0 || v != 0);

        // Update animator parameter
        animator.SetBool("IsMoving", isMoving);
    }
}
