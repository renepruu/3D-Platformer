using UnityEngine;

public class PortalSpawner : MonoBehaviour
{
    [Header("Portal Settings")]
    public GameObject portalPrefab;
    public KeyCode portalKey = KeyCode.E;
    public float spawnDistance = 3f;
    public float maxSpawnDistance = 20f;
    public LayerMask placementMask; // surfaces we can place the portal on

    [Tooltip("Vertical offset applied to the spawned portal position. Use negative values to place the portal lower. Default: -0.15")]
    public float spawnHeightOffset = -0.15f;

    [Header("World Offset")]
    [Tooltip("Offset between base world and parallel world. Example: (0,1000,0).")]
    public Vector3 worldOffset = new Vector3(0f, 1000f, 0f);

    private Camera _cam;

    // Keep track of the last spawned portals so we can replace them
    private GameObject _portalA;
    private GameObject _portalB;

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

        // Keep portal roughly at player height, with a small downward offset so you can
        // walk through without needing to jump.
        position.y = origin.y + spawnHeightOffset;

        // Face the portal toward the player, then rotate 90 degrees so the
        // visual door plane is oriented correctly.
        Quaternion rotation = Quaternion.LookRotation(-direction, Vector3.up) * Quaternion.Euler(0f, 90f, 0f);

        // Destroy previous portals if they exist
        if (_portalA != null)
            Destroy(_portalA);
        if (_portalB != null)
            Destroy(_portalB);

        // Spawn entrance portal at desired position
        _portalA = Instantiate(portalPrefab, position, rotation);

        // Determine whether we are currently in the base world or the
        // parallel world, based on height. If below halfway to the offset,
        // assume base; otherwise assume parallel.
        bool inBaseWorld = transform.position.y < worldOffset.y * 0.5f;
        Vector3 offset = inBaseWorld ? worldOffset : -worldOffset;

        // Spawn exit portal in the other world at the same local XZ coords.
        Vector3 exitPos = position + offset;
        _portalB = Instantiate(portalPrefab, exitPos, rotation);

        // Wire up Portal components so they know about each other
        var portalCompA = _portalA.GetComponent<Portal>();
        var portalCompB = _portalB.GetComponent<Portal>();
        if (portalCompA != null && portalCompB != null)
        {
            portalCompA.linkedPortal = _portalB.transform;
            portalCompB.linkedPortal = _portalA.transform;
        }

        // If the prefab has PortalView (same-scene visual effect), also link it
        var viewA = _portalA.GetComponent<PortalView>();
        var viewB = _portalB.GetComponent<PortalView>();
        if (viewA != null)
            viewA.linkedPortal = _portalB.transform;
        if (viewB != null)
            viewB.linkedPortal = _portalA.transform;
    }
}
