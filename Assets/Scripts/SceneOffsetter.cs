using UnityEngine;

/// <summary>
/// Offsets this GameObject (and thus all its children) by a fixed amount
/// when the scene loads. Attach it to a root object in the parallel scene
/// and make all level geometry children of that root.
///
/// Example: set offset to (0, 1000, 0) to move the whole map high above
/// the base world so they don't overlap in view space.
/// </summary>
public class SceneOffsetter : MonoBehaviour
{
    [Tooltip("World-space offset to apply to this scene's root.")]
    public Vector3 offset = new Vector3(0f, 1000f, 0f);

    private void Awake()
    {
        transform.position += offset;
    }
}
