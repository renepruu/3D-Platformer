using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    [Tooltip("Optional: limit which tag can use this portal (e.g. 'Player')")] 
    public string allowedTag = "Player";

    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true; // ensure trigger so player can walk through
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(allowedTag) && !other.CompareTag(allowedTag))
            return;

        if (PortalSceneManager.Instance == null)
        {
            Debug.LogWarning("Portal used but no PortalSceneManager in scene.");
            return;
        }

        // Use THIS portal's transform as the anchor position
        // so we always spawn at the door location in the other scene.
        PortalSceneManager.Instance.SwitchWorld(transform);
    }
}
