using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using NUnit.Framework.Constraints;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar;
    //public Scene scene;
    public TMP_Text Ltext;

    void Start()
    {
        StartCoroutine(LoadAsync());
        StartCoroutine(textAnim());
    }

    IEnumerator LoadAsync()
    {

        yield return new WaitForSeconds(0.25f);

        string nextScene = PlayerPrefs.GetString("MainGame");
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null)
                progressBar.value = progress;

            if (operation.progress >= 0.9f)
                operation.allowSceneActivation = true;

            yield return null;
        }
    }

    IEnumerator textAnim()
    {
        Ltext.text = "Loading..";
        yield return new WaitForSeconds(0.5f);
        Ltext.text = "Loading...";
        yield return new WaitForSeconds(0.5f);
        Ltext.text = "Loading.";
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(textAnim());

    }
}
