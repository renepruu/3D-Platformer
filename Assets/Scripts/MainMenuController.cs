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
        SceneManager.LoadScene("SettingsMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game closed!");
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
