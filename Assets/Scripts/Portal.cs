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

        // Determine the player's root transform from the collider that entered
        Transform playerRoot = other.attachedRigidbody != null
            ? other.attachedRigidbody.transform.root
            : other.transform.root;

        // Save the player's position/rotation so they appear at the same
        // world coordinates in the other scene.
        PortalSceneManager.Instance.SwitchWorld(playerRoot);
    }
}
