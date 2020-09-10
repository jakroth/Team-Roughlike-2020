using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public Image imageToFade;
    public float fadeSpeed = 2f;
    public bool isFading = false;

    IEnumerator popupRoutine;

    void Start() 
    {
        imageToFade.color = SetAlpha(imageToFade.color, 0);
        imageToFade.gameObject.SetActive(false);
    }

    //The update void is only for testing purposes
    void Update()
    {
        if(Input.GetKey(KeyCode.I) && !isFading)
        {
            isFading = true;
            FadeIn();
        }

        if(Input.GetKey(KeyCode.O) && !isFading)
        {
            isFading = true;
            FadeOut();
        }
    }

    public void FadeIn() 
    {
        StartCoroutine(FadeTo(true, fadeSpeed));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeTo(false, fadeSpeed));
    }

    public void FadeInAndOut()
    {
        StartCoroutine(FadeInAndOut(fadeSpeed, 2f));
    }

    private IEnumerator FadeTo(bool fadeIn, float aTime)
    {
        imageToFade.gameObject.SetActive(true);
        float alpha = fadeIn == true ? 0f : 1f;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / (fadeIn == true ? aTime : aTime / 2f))
        {
            Color newColor = new Color(0, 0, 0, Mathf.Lerp(alpha,fadeIn == true ? 1f : 0f,t));
            imageToFade.color = newColor;
            yield return null;
        }

        if(!fadeIn)
        {
            imageToFade.color = SetAlpha(imageToFade.color, 0);
            imageToFade.gameObject.SetActive(false);
        }

        isFading = false;
    }

    IEnumerator FadeInAndOut(float aTime, float waitTime) 
    {
        imageToFade.gameObject.SetActive(true);
        float alpha = 0f;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(0, 0, 0, Mathf.Lerp(alpha,1f,t));
            imageToFade.color = newColor;
            yield return null;
        }

        yield return new WaitForSeconds(waitTime);

        alpha = 1f;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / (aTime / 2))
        {
            Color newColor = new Color(0, 0, 0, Mathf.Lerp(alpha,0f,t));
            imageToFade.color = newColor;
            yield return null;
        }
        imageToFade.color = SetAlpha(imageToFade.color, 0);
        imageToFade.gameObject.SetActive(false);

    }

    Color SetAlpha(Color color, float a) 
    {
        color.a = a;

        return color;
    }
}
