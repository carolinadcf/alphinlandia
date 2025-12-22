using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // main, pause and settings menu references
    [SerializeField] public GameObject mainMenu;
    [SerializeField] public GameObject pauseMenu;
    [SerializeField] public GameObject settingsMenu;

    private void Start()
    {
        // ensure menus are closed at start
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        if (settingsMenu != null)
            settingsMenu.SetActive(false);
    }

    // Toggle pause menu on Escape key press
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    // start the game
    public void StartGame()
    {
        if (mainMenu != null)
        {
            mainMenu.SetActive(false);
            // load the first level
            SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu != null)
        {
            bool isActive = pauseMenu.activeSelf;
            pauseMenu.SetActive(!isActive);
            // pause the game when the menu is active
            Time.timeScale = isActive ? 1 : 0;
            // hide settings menu when pause is active
            if (settingsMenu != null && !isActive)
            {
                settingsMenu.SetActive(false);
            }
        }
    }

    public void ToggleSettingsMenu()
    {
        if (settingsMenu != null)
        {
            bool isActive = settingsMenu.activeSelf;
            settingsMenu.SetActive(!isActive);
            // toggle pause menu visibility
            TogglePauseMenu();
        }
    }

    public void ResumeGame()
    {
        if (pauseMenu != null && pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
        // If running in the Unity editor, stop playing
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Reload the current active scene
    public void RestartLevel()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    // Load the main menu scene (assuming it's at index 0)
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

}


