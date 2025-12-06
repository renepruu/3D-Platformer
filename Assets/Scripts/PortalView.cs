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
        if (portalCamera != null && portalCamera.targetTexture != null && screenRenderer != null)
        {
            // Ensure the portal screen shows whatever the portal camera renders.
            screenRenderer.material.mainTexture = portalCamera.targetTexture;
        }
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
        Transform srcPortal = transform;          // this portal
        Transform dstPortal = linkedPortal;       // other portal
        Transform cam = mainCamera.transform;
        Transform portalCam = portalCamera.transform;

        // 1) Express main camera position/rotation in this portal's local space.
        Vector3 camLocalPos = srcPortal.InverseTransformPoint(cam.position);
        Quaternion camLocalRot = Quaternion.Inverse(srcPortal.rotation) * cam.rotation;

        // 2) Mirror through the portal plane: flip forward + sideways (180° around up).
        camLocalPos = new Vector3(-camLocalPos.x, camLocalPos.y, -camLocalPos.z);
        camLocalRot = Quaternion.AngleAxis(180f, Vector3.up) * camLocalRot;

        // 3) Transform that local pose into the linked portal's world space.
        portalCam.position = dstPortal.TransformPoint(camLocalPos);
        portalCam.rotation = dstPortal.rotation * camLocalRot;

        // Match FOV and clip plane so the view feels correct.
        portalCamera.fieldOfView = mainCamera.fieldOfView;
        portalCamera.nearClipPlane = mainCamera.nearClipPlane;
        portalCamera.farClipPlane = mainCamera.farClipPlane;
    }
}
