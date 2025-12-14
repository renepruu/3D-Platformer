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

    [Tooltip("Optional: limit which tag can use this portal (e.g. 'Player').")]
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

        // IMPORTANT: use FULL 3D distance (prevents triggering the “other world” portal
        // just because it shares the same XZ). But use the CharacterController center,
        // not the player transform (which is usually at the feet), otherwise you may need
        // to jump to get close enough in Y.
        Vector3 playerProbePos = player.position;
        var cc = player.GetComponent<CharacterController>();
        if (cc != null)
            playerProbePos = cc.bounds.center;

        Vector3 delta = playerProbePos - transform.position;
        if (delta.sqrMagnitude <= triggerRadius * triggerRadius)
        {
            TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        // Make sure we place the player OUTSIDE the trigger radius of the destination portal,
        // otherwise you can immediately re-trigger.
        float safeOffset = Mathf.Max(exitOffset, triggerRadius + 0.1f);

        Vector3 exitPos = linkedPortal.position + linkedPortal.forward * safeOffset;
        Quaternion exitRot = linkedPortal.rotation;

        // CharacterController can fight direct transform moves; temporarily disable it.
        var cc = player.GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        player.SetPositionAndRotation(exitPos, exitRot);

        if (cc != null)
            cc.enabled = true;

        Debug.Log($"[Portal] Teleported player from {name} to {linkedPortal.name}");
    }
}
