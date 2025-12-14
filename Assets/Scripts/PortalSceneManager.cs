using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalSceneManager : MonoBehaviour
{
    public static PortalSceneManager Instance { get; private set; }

    [Header("Current Scene (read-only)")]
    [SerializeField] private string _currentScene;

    [Header("Scenes to preload additively (optional)")]
    [Tooltip("Names of scenes (from Build Settings) that should be loaded additively on startup.")]
    public string[] additiveScenesToLoad;

    [Header("When to preload")]
    [Tooltip("If empty, additive scenes will be ensured on every SINGLE scene load. If set, only these active scenes will trigger the preload (e.g. 'SceneWithAssets').")]
    public string[] preloadAdditivesWhenActiveSceneIs;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

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

        // Optionally preload any additional scenes additively so things like
        // parallel worlds are present for visual portals.
        EnsureAdditiveScenesLoaded();

        Debug.Log($"[PortalSceneManager] Active scene at startup: '{_currentScene}'");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Keep current scene in sync after menu/game transitions.
        // (For LoadSceneMode.Single, the loaded scene is effectively the new "main" scene.)
        if (mode == LoadSceneMode.Single)
        {
            _currentScene = scene.name;
            Debug.Log($"[PortalSceneManager] Active scene changed to: '{_currentScene}'");

            if (ShouldPreloadForScene(_currentScene))
                EnsureAdditiveScenesLoaded();
        }
    }

    private bool ShouldPreloadForScene(string activeSceneName)
    {
        if (preloadAdditivesWhenActiveSceneIs == null || preloadAdditivesWhenActiveSceneIs.Length == 0)
            return true;

        foreach (var s in preloadAdditivesWhenActiveSceneIs)
        {
            if (string.IsNullOrWhiteSpace(s))
                continue;
            if (string.Equals(s, activeSceneName, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    public void EnsureAdditiveScenesLoaded()
    {
        if (additiveScenesToLoad == null || additiveScenesToLoad.Length == 0)
            return;

        foreach (var sceneName in additiveScenesToLoad)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
                continue;

            var scene = SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded)
                continue;

            Debug.Log($"[PortalSceneManager] Loading additive scene '{sceneName}'...");
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }

    public string GetCurrentSceneName() => _currentScene;
}
