using UnityEngine;

public class PortalSpawner : MonoBehaviour
{
    [Header("Portal Settings")]
    public GameObject portalPrefab;
    public KeyCode portalKey = KeyCode.E;
    public float spawnDistance = 3f;
    public float maxSpawnDistance = 20f;
    public LayerMask placementMask; // surfaces we can place the portal on

    private Camera _cam;

    // Keep track of the last spawned portal so we can replace it
    private GameObject _currentPortal;

    private void Start()
    {
        _cam = GetComponentInChildren<Camera>();
        if (_cam == null)
        {
            Debug.LogWarning("PortalSpawner: no Camera found in children. Raycast will be from player forward.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(portalKey))
        {
            TrySpawnPortal();
        }
    }

    private void TrySpawnPortal()
    {
        if (portalPrefab == null)
        {
            Debug.LogWarning("PortalSpawner: no portalPrefab assigned.");
            return;
        }

        // Always spawn straight in front of where the player is looking,
        // at a fixed distance, slightly above the ground.
        // For a third-person setup, use the player's facing direction,
        // not the camera (which is usually behind the player).
        Vector3 origin = transform.position + Vector3.up * 1.0f; // player chest height
        Vector3 direction = transform.forward;                    // where the character is facing

        Vector3 position = origin + direction.normalized * spawnDistance;

        // Keep portal roughly at player height
        position.y = origin.y;

        // Face the portal toward the player, then rotate 90 degrees so the
        // visual door plane is oriented correctly.
        Quaternion rotation = Quaternion.LookRotation(-direction, Vector3.up) * Quaternion.Euler(0f, 90f, 0f);

        // Destroy previous portal if it exists
        if (_currentPortal != null)
        {
            Destroy(_currentPortal);
        }

        _currentPortal = Instantiate(portalPrefab, position, rotation);
    }
}
