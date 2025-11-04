using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSettings : MonoBehaviour
{
    [Header("Panels")]
    public GameObject visualpanel;
    public GameObject soundspanel;
    public GameObject controlpanel;

    private Resolution[] resolutions;

    #region PANEL SWITCHES
    public void OpenVisual()
    {
        visualpanel.SetActive(true);
        soundspanel.SetActive(false);
        controlpanel.SetActive(false);
    }

    public void OpenSounds()
    {
        visualpanel.SetActive(false);
        soundspanel.SetActive(true);
        controlpanel.SetActive(false);
    }

    public void OpenControls()
    {
        visualpanel.SetActive(false);
        soundspanel.SetActive(false);
        controlpanel.SetActive(true);
    }
    #endregion

    #region RESOLUTION / QUALITY / FULLSCREEN

    public void InitResolutionDropdown(Dropdown dropdown)
    {
        dropdown.ClearOptions();

        resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        dropdown.AddOptions(options);
        dropdown.value = currentResolutionIndex;
        dropdown.RefreshShownValue();

        Debug.Log($"[SettingsMenu] Résolutions disponibles : {resolutions.Length}. Actuelle : {Screen.currentResolution.width}x{Screen.currentResolution.height}");
    }

    public void SetResolution(int index)
    {
        if (resolutions == null || resolutions.Length == 0)
            resolutions = Screen.resolutions;

        Resolution res = resolutions[index];
       
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(res.width, res.height, false);
        Debug.Log($"[SettingsMenu] Résolution changée (fenêtré) : {res.width}x{res.height}");

      
        StartCoroutine(ReapplyFullscreen());
    }

    private IEnumerator ReapplyFullscreen()
    {
        yield return new WaitForSeconds(0.2f);
        if (Screen.fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Debug.Log("[SettingsMenu] Plein écran réactivé après changement de résolution");
        }
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        Debug.Log($"[SettingsMenu] Qualité changée -> {QualitySettings.names[index]}");
    }

    public void SetFullscreen(bool isFullscreen)
    {
        //Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.fullScreen = isFullscreen;
        Debug.Log($"[SettingsMenu] Mode plein écran -> {(isFullscreen ? "Activé" : "Désactivé")}");
    }

    #endregion

    private void Start()
    {
        Dropdown[] dropdowns = GetComponentsInChildren<Dropdown>();

        foreach (Dropdown d in dropdowns)
        {
            string lower = d.name.ToLower();
            if (lower.Contains("resolution"))
                InitResolutionDropdown(d);
            else if (lower.Contains("quality"))
                InitQualityDropdown(d);
        }

        Debug.Log("[SettingsMenu] Initialisation du menu de paramètres terminée.");
    }

    private void InitQualityDropdown(Dropdown dropdown)
    {
        dropdown.ClearOptions();

        string[] names = QualitySettings.names;
        dropdown.AddOptions(new List<string>(names));
        dropdown.value = QualitySettings.GetQualityLevel();
        dropdown.RefreshShownValue();

        Debug.Log($"[SettingsMenu] Dropdown qualités initialisé ({names.Length} niveaux). Niveau actuel : {QualitySettings.names[dropdown.value]}");
    }
}
