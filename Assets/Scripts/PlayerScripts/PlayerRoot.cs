using UnityEngine;

/// <summary>
/// Root component for the player hierarchy.
///
/// This makes the player (and its child camera) persistent across scene loads,
/// so you always have a valid gameplay camera after going through a portal.
/// Attach this to the top-level Player object (the parent of your PlayerCamera).
/// </summary>
[DisallowMultipleComponent]
public class PlayerRoot : MonoBehaviour
{
    private static PlayerRoot _instance;

    private void Awake()
    {
        // If another PlayerRoot already exists, destroy this one to avoid
        // duplicates when returning to a scene that also has a Player.
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
