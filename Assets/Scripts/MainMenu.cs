using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject creditpanel;
    public GameObject settingsPanel;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void PlayGame()
    {
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        // Optional fade animation
        yield return new WaitForSeconds(0.5f);

        // 1. Store the next scene name in PlayerPrefs
        PlayerPrefs.SetString("MainGame", "MainGame");
        PlayerPrefs.Save();

        // 2. Load the loading screen scene instead of MainGame
        SceneManager.LoadScene("LoadingScreen");
    }


    public void OpenSettings()
    {
        creditpanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        creditpanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game...");

        #if UNITY_EDITOR
        // Arrête le mode Play si on est dans l’éditeur
              UnityEditor.EditorApplication.isPlaying = false;
        #else
          // Ferme le jeu si on est en build
          Application.Quit();
        #endif
    }
}
