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
        // petite anim possible ici (fade)
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainGame");
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
        //Debug.Log("Quit Game...");
        Application.Quit();
    }
}
