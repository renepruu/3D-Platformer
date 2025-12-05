using UnityEngine;

public class MovementModeLoader : MonoBehaviour
{
    private PlayerMovement classic;
    private PlayerMovementStrafe strafe;

    private void Awake()
    {
        classic = GetComponent<PlayerMovement>();
        strafe = GetComponent<PlayerMovementStrafe>();

        int mode = PlayerPrefs.GetInt("MovementMode", 1); // default = strafe

        if (mode == 0)
        {
            classic.enabled = true;
            strafe.enabled = false;
        }
        else
        {
            classic.enabled = false;
            strafe.enabled = true;
        }
    }
}
