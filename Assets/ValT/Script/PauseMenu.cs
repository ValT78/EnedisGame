using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public GameObject confirmMenuUI;
    public GameObject grayBackgroundUI;

    [Header("Sliders")]
    public Image musicFillBar; // Image remplie pour la musique
    public Image sfxFillBar;   // Image remplie pour les effets spéciaux
    public Image sensitivityFillBar; // Image remplie pour la sensibilité

    [Header("Texts")]
    public TextMeshProUGUI musicValueText; // Texte pour afficher la valeur musique
    public TextMeshProUGUI sfxValueText;   // Texte pour les effets spéciaux
    public TextMeshProUGUI sensitivityValueText; // Texte pour la sensibilité

    [Header("Settings")]
    [Range(0, 1)] public float musicValue; // Valeur initiale pour la musique
    [Range(0, 1)] public float sfxValue;   // Valeur initiale pour les effets spéciaux
    [Range(0, 1)] public float sensitivityValue; // Valeur initiale pour la sensibilité


    void Start()
    {
        // Assure que le menu est désactivé au début
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        grayBackgroundUI.SetActive(false);

        // Initialiser les sliders avec les valeurs par défaut
        UpdateMusicSlider(musicValue);
        UpdateSFXSlider(sfxValue);
        UpdateSensitivitySlider(sensitivityValue);
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

    public void UpdateMusicSlider(float value)
    {
        musicValue = Mathf.Clamp01(value); // S'assurer que la valeur est entre 0 et 1
        musicFillBar.fillAmount = musicValue; // Mettre à jour le remplissage
        musicValueText.text = Mathf.RoundToInt(musicValue * 100) + "%"; // Mettre à jour le texte
        // Appliquer la valeur au volume de la musique
        AudioManager.Instance.musicSource.volume = musicValue; // Exemple, remplace par ton propre gestionnaire de son
    }

    public void UpdateSFXSlider(float value)
    {
        sfxValue = Mathf.Clamp01(value);
        sfxFillBar.fillAmount = sfxValue;
        sfxValueText.text = Mathf.RoundToInt(sfxValue * 100) + "%";
        AudioManager.Instance.soundEffectSource.volume = sfxValue; // Exemple, remplace par ton propre gestionnaire de son
    }

    public void UpdateSensitivitySlider(float value)
    {
        sensitivityValue = Mathf.Clamp01(value);
        sensitivityFillBar.fillAmount = sensitivityValue;
        sensitivityValueText.text = Mathf.RoundToInt(sensitivityValue * 100) + "%";
        // Appliquer la valeur à la sensibilité de la souris
        FirstPersonController.Instance.mouseSensitivity = sensitivityValue * 200;
    }
}
