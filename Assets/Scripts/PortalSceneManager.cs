using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalSceneManager : MonoBehaviour
{
    public static PortalSceneManager Instance { get; private set; }

    [Header("Scene Names (must match Build Settings)")]
    public string sceneA = "World_A";
    public string sceneB = "World_B";

    [Header("Visual Portal Setup (optional)")]
    [Tooltip("Camera that renders the other scene into a RenderTexture for visual portals (legacy, optional).")]
    public Camera portalCamera;
    [Tooltip("RenderTexture used by visual portals. Assign this to portalCamera.targetTexture (legacy, optional).")]
    public RenderTexture portalTexture;

    [Header("World Layer Setup (optional but recommended)")]
    [Tooltip("Layer mask for the 'A' world (what the main camera should see when in sceneA).")]
    public LayerMask worldALayers;
    [Tooltip("Layer mask for the 'B' world (what the main camera should see when in sceneB).")]
    public LayerMask worldBLayers;
    [Tooltip("The main gameplay camera (player camera).")]
    public Camera mainCamera;

    [HideInInspector] public Vector3 lastPlayerPosition;
    [HideInInspector] public Quaternion lastPlayerRotation;

    private string _currentScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _currentScene = SceneManager.GetActiveScene().name;

        CacheMainCamera();

        // Ensure the legacy portal camera and texture survive scene loads as well, if used.
        if (portalCamera != null)
        {
            DontDestroyOnLoad(portalCamera.gameObject);

            if (portalTexture != null && portalCamera.targetTexture == null)
                portalCamera.targetTexture = portalTexture;
        }
    }

    private void OnEnable()
    {
        // In case the scene loads before the player, try to grab the main camera
        // again when this object is re-enabled.
        CacheMainCamera();
    }

    private void CacheMainCamera()
    {
        if (mainCamera != null)
            return;

        if (Camera.main != null)
            mainCamera = Camera.main;
    }

    public string GetCurrentSceneName() => _currentScene;

    public string GetOtherSceneName()
    {
        if (_currentScene == sceneA)
            return sceneB;
        if (_currentScene == sceneB)
            return sceneA;
        return sceneA;
    }

    /// <summary>
    /// Make sure the "other" scene is loaded additively so visual portals can
    /// render it. Does nothing if it's already loaded.
    /// </summary>
    public void EnsureOtherSceneLoaded()
    {
        // Visual cross-scene portals previously used additive loading to keep the
        // other world around. This caused multiple copies of scenes to pile up
        // and inconsistent behaviour. For now we disable additive loading and
        // keep a single active scene at a time.
        return;
    }

    public void SwitchWorld(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("PortalSceneManager.SwitchWorld called with null player");
            return;
        }

        lastPlayerPosition = playerTransform.position;
        lastPlayerRotation = playerTransform.rotation;

        Debug.Log($"[PortalSceneManager] Saving player pos {lastPlayerPosition} rot {lastPlayerRotation.eulerAngles}");

        string targetScene;
        if (_currentScene == sceneA)
        {
            targetScene = sceneB;

            // Switch main camera to render the B world layers when we arrive there.
            if (mainCamera != null && worldBLayers.value != 0)
                mainCamera.cullingMask = worldBLayers;
        }
        else if (_currentScene == sceneB)
        {
            targetScene = sceneA;

            // Switch main camera to render the A world layers when we arrive there.
            if (mainCamera != null && worldALayers.value != 0)
                mainCamera.cullingMask = worldALayers;
        }
        else
        {
            targetScene = sceneA;
        }

        Debug.Log($"Portaling from {_currentScene} to {targetScene} at {lastPlayerPosition}");

        _currentScene = targetScene;
        SceneManager.LoadScene(targetScene);
    }

    // Apply the last saved portal pose to the player root in the new scene.
    public void ApplySavedTransformToPlayer(Transform playerTransform)
    {
        if (playerTransform == null) return;

        Transform root = playerTransform.root;

        Debug.Log($"[PortalSceneManager] Applying saved portal pos {lastPlayerPosition} rot {lastPlayerRotation.eulerAngles} to root {root.name}");

        root.position = lastPlayerPosition;
        root.rotation = lastPlayerRotation;
    }
}
