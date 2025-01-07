using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public GameObject confirmMenuUI;
    public GameObject grayBackgroundUI;


    void Start()
    {
        // Assure que le menu est désactivé au début
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        grayBackgroundUI.SetActive(false);
    }

    void Update()
    {
        // Détection de l'action Pause via le nouveau Input System
        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameManager.Instance.isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        grayBackgroundUI.SetActive(true);
        GameManager.Instance.PauseGame(); // Appelle la fonction PauseGame du GameManager
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        grayBackgroundUI.SetActive(false);
        GameManager.Instance.ResumeGame(); // Appelle la fonction ResumeGame du GameManager
    }

    public void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        confirmMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void QuitGame()
    {
        pauseMenuUI.SetActive(false);
        confirmMenuUI.SetActive(true);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Assure que le jeu reprend avant de charger une nouvelle scène
        SceneManager.LoadScene("MainMenu"); // Remplace "MainMenu" par le nom de ta scène principale
    }
}
