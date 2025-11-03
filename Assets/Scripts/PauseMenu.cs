using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;
using StarterAssets;
using Cinemachine;



public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("Main Menus")]
    public GameObject pauseMenuUI;
    public GameObject MainUI;
    public GameObject settingsMenuUI;
    public GameObject ButtonPanelUI;

    [Header("Settings Panels")]
    public GameObject visualsPanel;
    public GameObject soundPanel;
    public GameObject controlsPanel;

    [Header("Visual Settings")]
    public Slider fovSlider;
    public Camera playerCamera; // pour caméra classique (optionnel)
    public CinemachineVirtualCamera playerVirtualCamera; // pour la Cinemachine
    public TMP_Dropdown qualityDropdown;



    [Header("Sound Settings")]
    public Slider volumeSlider;
    public AudioMixer mainMixer;

    [Header("Controls Settings")]
    public Slider sensitivitySlider;

    [Header("Input Actions")]
    public InputActionAsset inputActions;

    // Valeur actuelle de la sensibilité (stockée ici)
    public static float mouseSensitivity = 1f;

    void Start()
    {
        LoadSettings();

        // Ajouter les listeners
        if (fovSlider != null) fovSlider.onValueChanged.AddListener(ApplyFOV);
        if (volumeSlider != null) volumeSlider.onValueChanged.AddListener(ApplyVolume);
        if (sensitivitySlider != null) sensitivitySlider.onValueChanged.AddListener(ApplySensitivity);
        if (qualityDropdown != null) qualityDropdown.onValueChanged.AddListener(ApplyQuality);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                if (settingsMenuUI.activeSelf)
                    OpenPauseMenu();
                else
                    Resume();
            }
            else
                Pause();
        }
    }

    void LateUpdate()
    {
        if (GameIsPaused)
        {
            if (Cursor.lockState != CursorLockMode.None || !Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    // --------------------------
    // PAUSE / RESUME
    // --------------------------
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        ButtonPanelUI.SetActive(true);
        MainUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        PauseGame();

        inputActions.FindActionMap("Player").Disable();
    }

    public void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        MainUI.SetActive(true);
        ResumeGame();

        inputActions.FindActionMap("Player").Enable();
    }

    // --------------------------
    // PANELS
    // --------------------------
    public void OpenPauseMenu()
    {
        pauseMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
        ButtonPanelUI.SetActive(true);
        MainUI.SetActive(false);
    }

    public void OpenSettings()
    {
        pauseMenuUI.SetActive(true);
        ButtonPanelUI.SetActive(false);
        settingsMenuUI.SetActive(true);
        MainUI.SetActive(false);
        // OpenVisualsPanel();
    }

    public void OpenVisualsPanel()
    {
        visualsPanel.SetActive(true);
        soundPanel.SetActive(false);
        controlsPanel.SetActive(false);
        ButtonPanelUI.SetActive(false);
    }

    public void OpenSoundPanel()
    {
        visualsPanel.SetActive(false);
        soundPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void OpenControlsPanel()
    {
        visualsPanel.SetActive(false);
        ButtonPanelUI.SetActive(false);
        soundPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }

    // --------------------------
    // SETTINGS LOGIC
    // --------------------------
    public void ApplyFOV(float value)
    {
        
        if (playerCamera != null)
            playerCamera.fieldOfView = value;

        
        if (playerVirtualCamera != null)
            playerVirtualCamera.m_Lens.FieldOfView = value;

        PlayerPrefs.SetFloat("FOV", value);
    }


    public void ApplyVolume(float value)
    {
        if (mainMixer != null)
            mainMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);

        PlayerPrefs.SetFloat("Volume", value);
    }

    public void ApplySensitivity(float value)
    {
        mouseSensitivity = value;
        PlayerPrefs.SetFloat("Sensitivity", value);

       
        StarterAssetsInputs input = FindAnyObjectByType<StarterAssetsInputs>();
        if (input != null)
        {
            input.lookSensitivity = value;
        }
    }



    public void ApplyQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("Quality", index);
    }

    public void LoadSettings()
    {
       


        if (fovSlider != null)
        {
            float fov = PlayerPrefs.GetFloat("FOV", 60);
            fovSlider.value = fov;
            ApplyFOV(fov);
        }

        if (volumeSlider != null)
        {
            float vol = PlayerPrefs.GetFloat("Volume", 0.75f);
            volumeSlider.value = vol;
            ApplyVolume(vol);
        }

        if (sensitivitySlider != null)
        {
            float sens = PlayerPrefs.GetFloat("Sensitivity", 1f);
            sensitivitySlider.value = sens;
            ApplySensitivity(sens);
        }

        if (qualityDropdown != null)
        {
            int q = PlayerPrefs.GetInt("Quality", 2);
            qualityDropdown.value = q;
            ApplyQuality(q);
        }
    }
}
