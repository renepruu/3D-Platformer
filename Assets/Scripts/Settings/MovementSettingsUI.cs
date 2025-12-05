using UnityEngine;
using UnityEngine.UI;

public class MovementSettingsUI : MonoBehaviour
{
    public Toggle toggleClassic;   // PlayerMovement
    public Toggle toggleStrafe;    // PlayerMovementStrafe

    private void Start()
    {
        // Load previous choice
        int mode = PlayerPrefs.GetInt("MovementMode", 1);  // default 1 = Strafe

        toggleClassic.isOn = (mode == 0);
        toggleStrafe.isOn = (mode == 1);

        toggleClassic.onValueChanged.AddListener((on) =>
        {
            if (on) PlayerPrefs.SetInt("MovementMode", 0);
        });

        toggleStrafe.onValueChanged.AddListener((on) =>
        {
            if (on) PlayerPrefs.SetInt("MovementMode", 1);
        });
    }
}
