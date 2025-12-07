using UnityEngine;

public class Portal : MonoBehaviour
{
    [Tooltip("Transform of the player root (auto-found if left empty).")]
    public Transform player;

    [Tooltip("The other portal this one connects to.")]
    public Transform linkedPortal;

    [Tooltip("How close the player must be (in meters) to trigger teleport.")]
    public float triggerRadius = 1.0f;

    [Tooltip("How far in front of the exit portal to place the player.")]
    public float exitOffset = 1.5f;

    [Tooltip("Optional: limit which tag can use this portal (e.g. 'Player')")] 
    public string allowedTag = "Player";

    private void Awake()
    {
        if (player == null)
        {
            var pm = FindFirstObjectByType<PlayerMovement>();
            if (pm != null)
                player = pm.transform;
        }
    }

    private void Update()
    {
        if (player == null || linkedPortal == null)
            return;

        // Optional tag check
        if (!string.IsNullOrEmpty(allowedTag) && !player.CompareTag(allowedTag))
            return;

        Vector3 playerPos = player.position;
        Vector3 portalPos = transform.position;

        Vector2 a = new Vector2(playerPos.x, playerPos.z);
        Vector2 b = new Vector2(portalPos.x, portalPos.z);

        if (Vector2.SqrMagnitude(a - b) <= triggerRadius * triggerRadius)
        {
            TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        Vector3 exitPos = linkedPortal.position + linkedPortal.forward * exitOffset;
        Quaternion exitRot = linkedPortal.rotation;

        player.position = exitPos;
        player.rotation = exitRot;

        Debug.Log($"[Portal] Teleported player from {name} to {linkedPortal.name}");
    }
}
