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

        // Use the root transform in case the collider is on a child
        Transform playerRoot = other.attachedRigidbody != null
            ? other.attachedRigidbody.transform
            : other.transform.root;

        PortalSceneManager.Instance.SwitchWorld(playerRoot);
    }
}
