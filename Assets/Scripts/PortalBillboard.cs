using UnityEngine;

/// <summary>
/// Rotates this portal so that its front face always points toward a target
/// (typically the player camera). Use for billboard-style portals.
/// </summary>
public class PortalBillboard : MonoBehaviour
{
    [Tooltip("Transform to face. Leave empty to auto-find the player camera.")]
    public Transform target;

    [Tooltip("If true, only rotate around Y so the portal stays upright.")]
    public bool lockYAxis = true;

    [Tooltip("Flip facing direction if the wrong side is visible.")]
    public bool invertFacing = true;

    void LateUpdate()
    {
        if (target == null)
        {
            TryFindTarget();
            if (target == null)
                return;
        }

        Vector3 dir = target.position - transform.position;
        if (lockYAxis)
            dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        // Optionally flip so the visible portal face points at the player
        if (invertFacing)
            dir = -dir;

        transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }

    private void TryFindTarget()
    {
        // Prefer the camera attached to PlayerMovement, if any
        var player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            var cam = player.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                target = cam.transform;
                return;
            }
        }

        // Fallback to Camera.main
        if (Camera.main != null)
            target = Camera.main.transform;
    }
}
