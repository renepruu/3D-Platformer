using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalSceneManager : MonoBehaviour
{
    public static PortalSceneManager Instance { get; private set; }

    [Header("Scene Names (must match Build Settings)")]
    public string sceneA = "World_A";
    public string sceneB = "World_B";

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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentScene = scene.name;
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

        string targetScene;
        if (_currentScene == sceneA)
            targetScene = sceneB;
        else if (_currentScene == sceneB)
            targetScene = sceneA;
        else
            targetScene = sceneA;

        Debug.Log($"Portaling from {_currentScene} to {targetScene} at {lastPlayerPosition}");

        SceneManager.LoadScene(targetScene);
    }

    public void ApplySavedTransformToPlayer(Transform playerTransform)
    {
        if (playerTransform == null) return;

        playerTransform.position = lastPlayerPosition;
        playerTransform.rotation = lastPlayerRotation;
    }
}
