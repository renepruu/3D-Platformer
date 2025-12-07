using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("SceneWithAssets");
    }

    public void Settings()
    {
        // destroy persistent boss (if any) so its camera doesn't persist into the settings scene
        var boss = GameObject.Find("The Boss@Idle(1)");
        if (boss != null)
        {
            Debug.Log("[MainMenuController] Destroying persistent boss before opening Settings.");
            Destroy(boss);
        }
        SceneManager.LoadScene("SettingsMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game closed!");
    }
    public void LoadMainMenu()
    {
        // ensure persistent boss is removed to avoid duplicate cameras in the main menu
        var boss = GameObject.Find("The Boss@Idle(1)");
        if (boss != null) Destroy(boss);
        SceneManager.LoadScene("MainMenuScene");
    }
}
