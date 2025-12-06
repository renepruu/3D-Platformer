using UnityEngine;

public class PlayerPositionDisplay : MonoBehaviour
{
    [Tooltip("Player transform to track (e.g. your Player root)")]
    public Transform player;

    [Tooltip("Padding from the top-right corner in pixels")]
    public Vector2 padding = new Vector2(10f, 10f);

    void OnGUI()
    {
        if (player == null) return;

        Vector3 p = player.position;
        string text = $"Player Pos: X={p.x:F2}, Y={p.y:F2}, Z={p.z:F2}";

        // Use the default GUI label style for now
        GUIStyle style = GUI.skin.label;
        Vector2 size = style.CalcSize(new GUIContent(text));

        // Top-right corner
        float x = Screen.width - size.x - padding.x;
        float y = padding.y;

        GUI.Label(new Rect(x, y, size.x, size.y), text, style);
    }
}
