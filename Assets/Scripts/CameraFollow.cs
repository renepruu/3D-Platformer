using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;         // Reference to the player transform
    public Vector3 offset = new Vector3(0, 5, -10);  // Default camera position behind player
    public float smoothSpeed = 0.125f;  // Smoothing factor for motion

    void LateUpdate()
    {
        if (player == null) return;

        // Desired position (follow player with offset)
        Vector3 desiredPosition = player.position + offset;

        // Smoothly interpolate between current and desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;

        // Optional: make camera look at the player
        transform.LookAt(player);
    }
}
