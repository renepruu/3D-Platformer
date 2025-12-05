using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance; // Singleton instance

    public GameObject PauseMenu;
    private bool isPaused = false;


    void Awake()
    {
        // Check if instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    private void Pause()
    {
        PauseMenu.SetActive(true);
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void Resume()
    {

        PauseMenu.SetActive(false);
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }


    public void OpenSettings()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
        SceneManager.LoadScene("SettingsMenu");
    }

    public void OpenMenu()
    {
        // Reset time scale in case game was paused
        Time.timeScale = 1f;


        // Destroy this persistent PauseMenu
        Destroy(gameObject);

        // Load main menu scene
        SceneManager.LoadScene("MainMenuScene");
    }


}
