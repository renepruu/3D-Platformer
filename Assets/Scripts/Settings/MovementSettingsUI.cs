using UnityEngine;
using UnityEngine.UI;

public class MovementSettingsUI : MonoBehaviour
{
    public Toggle toggleClassic;   // PlayerMovement
    public Toggle toggleStrafe;    // PlayerMovementStrafe

    private bool isInternalChange = false;  // prevents infinite loops

    private void Start()
    {
        int mode = PlayerPrefs.GetInt("MovementMode", 1); // default 1 = strafe

        // Initialize toggle states
        toggleClassic.isOn = (mode == 0);
        toggleStrafe.isOn = (mode == 1);

        toggleClassic.onValueChanged.AddListener(OnClassicToggled);
        toggleStrafe.onValueChanged.AddListener(OnStrafeToggled);

        // Ensure one is always active at game start
        EnforceMinimumOneActive();
    }

    private void OnClassicToggled(bool isOn)
    {
        if (isInternalChange) return;

        if (isOn)
        {
            // Activate classic
            SetInternal(toggleStrafe, false);
            PlayerPrefs.SetInt("MovementMode", 0);
        }
        else
        {
            // Prevent both being off
            if (!toggleStrafe.isOn)
                SetInternal(toggleClassic, true);
        }
    }

    private void OnStrafeToggled(bool isOn)
    {
        if (isInternalChange) return;

        if (isOn)
        {
            // Activate strafe
            SetInternal(toggleClassic, false);
            PlayerPrefs.SetInt("MovementMode", 1);
        }
        else
        {
            // Prevent both being off
            if (!toggleClassic.isOn)
                SetInternal(toggleStrafe, true);
        }
    }

    // Helper to avoid infinite event loops
    private void SetInternal(Toggle toggle, bool value)
    {
        isInternalChange = true;
        toggle.isOn = value;
        isInternalChange = false;
    }

    private void EnforceMinimumOneActive()
    {
        if (!toggleClassic.isOn && !toggleStrafe.isOn)
        {
            // Default to strafe
            SetInternal(toggleStrafe, true);
            PlayerPrefs.SetInt("MovementMode", 1);
        }
    }
}
