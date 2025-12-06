using UnityEngine;

/// <summary>
/// Visual-only component for portals that show the OTHER scene.
/// Assumes PortalSceneManager knows which scene is current and can
/// ensure the other scene is loaded additively.
///
/// This does NOT handle teleporting; it only drives the portal camera
/// and assigns its RenderTexture to the portal screen.
/// </summary>
[RequireComponent(typeof(Collider))]
public class CrossScenePortalView : MonoBehaviour
{
    [Tooltip("MeshRenderer of the portal's screen quad.")]
    public MeshRenderer screenRenderer;

    [Tooltip("Optional: camera used to render the other scene. If null, one is created as a child.")]
    public Camera portalCamera;

    [Tooltip("Optional: RenderTexture for this portal. If null, one is created at runtime.")]
    public RenderTexture portalTexture;

    private Camera _mainCamera;
    private PortalSceneManager _mgr;

    private void Start()
    {
        _mgr = PortalSceneManager.Instance;
        _mainCamera = TryFindMainCamera();

        if (screenRenderer == null)
        {
            Debug.LogWarning($"[CrossScenePortalView] screenRenderer is not assigned on {name}.");
            enabled = false;
            return;
        }

        // Create / find a dedicated camera for this portal instance
        if (portalCamera == null)
        {
            portalCamera = GetComponentInChildren<Camera>();
            if (portalCamera == null)
            {
                var go = new GameObject("PortalViewCamera");
                go.transform.SetParent(transform, false);
                portalCamera = go.AddComponent<Camera>();
            }
        }

        // Create a RenderTexture if none assigned
        if (portalTexture == null)
        {
            portalTexture = new RenderTexture(Screen.width, Screen.height, 16)
            {
                name = $"PortalRT_{name}",
                autoGenerateMips = false
            };
            portalTexture.Create();
        }

        if (portalCamera.targetTexture == null)
            portalCamera.targetTexture = portalTexture;

        // Show this camera's view on the portal screen
        screenRenderer.material.mainTexture = portalCamera.targetTexture;
    }

    private void LateUpdate()
    {
        if (portalCamera == null)
            return;

        if (_mainCamera == null)
            _mainCamera = TryFindMainCamera();
        if (_mainCamera == null)
            return;

        // Make sure the other scene is loaded so the portal camera can see it.
        _mgr?.EnsureOtherSceneLoaded();

        // Mirror the main camera pose. Because both scenes are aligned copies,
        // this shows what you'd see from the same position in the other world.
        Transform camT = _mainCamera.transform;
        Transform portalCamT = portalCamera.transform;

        portalCamT.position = camT.position;
        portalCamT.rotation = camT.rotation;

        portalCamera.fieldOfView = _mainCamera.fieldOfView;
        portalCamera.nearClipPlane = _mainCamera.nearClipPlane;
        portalCamera.farClipPlane = _mainCamera.farClipPlane;

        // Render the OTHER world's layers, if PortalSceneManager is configured.
        if (_mgr != null)
        {
            // If current is sceneA → show worldB, else show worldA.
            if (_mgr.GetCurrentSceneName() == _mgr.sceneA && _mgr.worldBLayers.value != 0)
                portalCamera.cullingMask = _mgr.worldBLayers;
            else if (_mgr.GetCurrentSceneName() == _mgr.sceneB && _mgr.worldALayers.value != 0)
                portalCamera.cullingMask = _mgr.worldALayers;
        }
    }

    private Camera TryFindMainCamera()
    {
        var player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            var cam = player.GetComponentInChildren<Camera>();
            if (cam != null)
                return cam;
        }

        return Camera.main;
    }
}
