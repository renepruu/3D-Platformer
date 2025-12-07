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
        if (linkedPortal == null || mainCamera == null || portalCamera == null)
            return;

        UpdatePortalCameraPose();
    }

    /// <summary>
    /// Positions and orients the portal camera so that looking at this portal
    /// shows the correct perspective from the linked portal.
    /// </summary>
    private void UpdatePortalCameraPose()
    {
        if (linkedPortal == null || portalCamera == null)
            return;

        Transform srcPortal = transform;          // this portal
        Transform dstPortal = linkedPortal;       // other portal
        Transform cam = mainCamera.transform;
        Transform portalCam = portalCamera.transform;

        // We want a "see-through door" effect, not a mirror. So take the
        // camera's local offset from this portal and apply it to the linked
        // portal without mirroring.
        //
        // 1) Express main camera position relative to this portal.
        Vector3 camLocalPos = srcPortal.InverseTransformPoint(cam.position);
        Quaternion camLocalRot = Quaternion.Inverse(srcPortal.rotation) * cam.rotation;

        // 2) Apply that same local pose to the linked portal.
        portalCam.position = dstPortal.TransformPoint(camLocalPos);
        portalCam.rotation = dstPortal.rotation * camLocalRot;

        // Match FOV, clip planes, and culling mask so the view feels correct.
        portalCamera.fieldOfView = mainCamera.fieldOfView;
        portalCamera.nearClipPlane = mainCamera.nearClipPlane;
        portalCamera.farClipPlane = mainCamera.farClipPlane;
        portalCamera.cullingMask = mainCamera.cullingMask;
    }
}
