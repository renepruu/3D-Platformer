using UnityEngine;
public class PortalSceneManager : MonoBehaviour
{
    public static PortalSceneManager Instance { get; private set; }

    [Header("Current Scene (read-only)")]
    [SerializeField] private string _currentScene;

    [Header("Scenes to preload additively (optional)")]
    [Tooltip("Names of scenes (from Build Settings) that should be loaded additively on startup, e.g. 'SceneWithAssets(parallel)'.")]
    public string[] additiveScenesToLoad;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Optionally preload any additional scenes additively so things like
        // parallel worlds are present for visual portals.
        PreloadAdditiveScenes();

        Debug.Log($"[PortalSceneManager] Active scene at startup: '{_currentScene}'");
    }

    private void PreloadAdditiveScenes()
    {
        if (additiveScenesToLoad == null)
            return;

        foreach (var sceneName in additiveScenesToLoad)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
                continue;

            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded)
            {
                Debug.Log($"[PortalSceneManager] Additive scene '{sceneName}' already loaded.");
                continue;
            }

            Debug.Log($"[PortalSceneManager] Loading additive scene '{sceneName}'...");
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
                sceneName,
                UnityEngine.SceneManagement.LoadSceneMode.Additive
            );
        }
    }

    public string GetCurrentSceneName() => _currentScene;
}
