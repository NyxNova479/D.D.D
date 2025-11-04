using UnityEngine;
using System;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public float fadeDuration = 0.75f; 
    private float alpha = 0f;
    private bool isFading = false;
    private Texture2D blackTexture;

   
    public Action OnFadeToBlack;   
    public Action OnFadeComplete;  

    void Start()
    {
        blackTexture = new Texture2D(1, 1);
        blackTexture.SetPixel(0, 0, Color.black);
        blackTexture.Apply();
    }

    void OnGUI()
    {
        if (isFading)
        {
            Color guiColor = GUI.color;
            guiColor.a = alpha;
            GUI.color = guiColor;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture);
        }
    }

    public void StartFade()
    {
        if (!isFading)
            StartCoroutine(FadeInOut());
    }

    private IEnumerator FadeInOut()
    {
        isFading = true;

       
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        alpha = 1f;
        OnFadeToBlack?.Invoke(); 

       
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        alpha = 0f;

        isFading = false;
        OnFadeComplete?.Invoke(); 
    }
}
