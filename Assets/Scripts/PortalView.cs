using UnityEngine;

/// <summary>
/// Renders a view through this portal into the area around a linked portal,
/// using a secondary camera and a RenderTexture.
/// 
/// This is for SAME-SCENE portals only (both portals in one Unity scene).
/// </summary>
[RequireComponent(typeof(Collider))]
public class PortalView : MonoBehaviour
{
    [Header("Portal Setup")]
    [Tooltip("The other portal this one looks through to.")]
    public Transform linkedPortal;

    [Tooltip("The main gameplay camera (what the player sees).")]
    public Camera mainCamera;

    [Tooltip("Camera that renders the view from the linked portal.")]
    public Camera portalCamera;

    [Tooltip("MeshRenderer for the portal screen/quad.")]
    public MeshRenderer screenRenderer;

    private void Start()
    {
        // Auto-find main camera if not wired.
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (portalCamera != null && portalCamera.targetTexture != null && screenRenderer != null)
        {
            // Ensure the portal screen shows whatever the portal camera renders.
            screenRenderer.material.mainTexture = portalCamera.targetTexture;
        }

        Debug.Log($"[PortalView] {name} mainCamera={(mainCamera ? mainCamera.name : "null")} portalCamera={(portalCamera ? portalCamera.name : "null")} linkedPortal={(linkedPortal ? linkedPortal.name : "null")}");
    }

    private void LateUpdate()
    {
        if (linkedPortal == null || portalCamera == null)
            return;

        UpdatePortalCameraPose();
    }

    /// <summary>
    /// Positions and orients the portal camera so that looking at this portal
    /// shows a FIXED view from the linked portal, like a CCTV camera mounted
    /// in the centre of the other door, always looking straight out.
    ///
    /// This is intentionally independent of the player's camera position.
    /// </summary>
    private void UpdatePortalCameraPose()
    {
        Transform dstPortal = linkedPortal;       // other portal
        Transform portalCam = portalCamera.transform;

        // Place the camera at the centre of the other portal, slightly in
        // front of the surface so it doesn't clip into the geometry.
        const float forwardOffset = 0.05f;
        portalCam.position = dstPortal.position + dstPortal.forward * forwardOffset;
        portalCam.rotation = dstPortal.rotation;

        // Optionally match FOV / clip planes to the main camera so it feels
        // similar, but do NOT copy its position or rotation.
        if (mainCamera != null)
        {
            portalCamera.fieldOfView = mainCamera.fieldOfView;
            portalCamera.nearClipPlane = mainCamera.nearClipPlane;
            portalCamera.farClipPlane = mainCamera.farClipPlane;
            portalCamera.cullingMask = mainCamera.cullingMask;
        }
    }
}
