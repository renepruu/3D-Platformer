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

    private RenderTexture _runtimeTexture;
    private bool _hasRenderedOnce;

    private void Start()
    {
        // Auto-find main camera if not wired.
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (portalCamera == null || screenRenderer == null)
        {
            Debug.LogWarning($"[PortalView] Missing portalCamera or screenRenderer on {name}.");
            enabled = false;
            return;
        }

        // IMPORTANT: the prefab uses a project asset RenderTexture.
        // If you spawn two portals, both portal cameras will render into the SAME texture
        // unless we clone it per instance. That causes the portal screen to often show the
        // wrong view (whichever camera rendered last), which looks like you need to press
        // the key twice to get the correct portal.
        EnsureUniqueRenderTexture();

        // Ensure the portal screen shows whatever the portal camera renders.
        screenRenderer.material.mainTexture = portalCamera.targetTexture;

        // Don't let the portal camera render until we have a linked portal AND have positioned it.
        // This prevents showing a stale texture / the wrong pose on the first frame.
        portalCamera.enabled = false;

        Debug.Log($"[PortalView] {name} mainCamera={(mainCamera ? mainCamera.name : "null")} portalCamera={(portalCamera ? portalCamera.name : "null")} linkedPortal={(linkedPortal ? linkedPortal.name : "null")} rt={(portalCamera.targetTexture ? portalCamera.targetTexture.name : "null")}");
    }

    private void LateUpdate()
    {
        if (linkedPortal == null || portalCamera == null)
            return;

        // In case the main camera changes (menus, respawns, etc.)
        if (mainCamera == null)
            mainCamera = Camera.main;

        UpdatePortalCameraPose();

        // Enable and force an immediate first render once we're correctly positioned.
        if (!portalCamera.enabled)
        {
            portalCamera.enabled = true;
            if (!_hasRenderedOnce)
            {
                portalCamera.Render();
                _hasRenderedOnce = true;
            }
        }
    }

    private void OnDestroy()
    {
        if (_runtimeTexture != null)
        {
            if (portalCamera != null)
                portalCamera.targetTexture = null;

            _runtimeTexture.Release();
            Destroy(_runtimeTexture);
            _runtimeTexture = null;
        }
    }

    private void EnsureUniqueRenderTexture()
    {
        // If the camera already has a target texture (likely an asset), clone it so
        // each portal instance has its own RT.
        var src = portalCamera.targetTexture;
        RenderTextureDescriptor desc;

        if (src != null)
        {
            desc = src.descriptor;
        }
        else
        {
            // Fallback if someone forgot to assign a target RT on the camera.
            desc = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.ARGB32, 16);
        }

        _runtimeTexture = new RenderTexture(desc)
        {
            name = $"PortalRT_{name}_{GetInstanceID()}",
            autoGenerateMips = false
        };
        _runtimeTexture.Create();

        portalCamera.targetTexture = _runtimeTexture;
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
