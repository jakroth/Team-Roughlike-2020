using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class FadeAudioSource 
{
    public static IEnumerator StartFade(AudioSource audioSource, float dur, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while(currentTime < dur)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / dur);
            yield return null;
        }
        yield break;
    }
}

public class MusicController : MonoBehaviour
{

    //plays music from a persistent audio source with this script attached but changes music based
    public AudioSource audioSource;
    public AudioClip bossMusic;
    public AudioClip enemyMusic;
    public AudioClip backgroundMusic;
    public AudioClip titleMusic;

    public static MusicController instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);

        int index = SceneManager.GetActiveScene().buildIndex;
        switch(index)
        {
            case 0:
                audioSource.clip = titleMusic;
                break;
            case 1:
                audioSource.clip = backgroundMusic;
                break;
            case 2:
                audioSource.clip = enemyMusic;
                break;
            case 3:
                audioSource.clip = bossMusic;
                break;
            case 4:
                audioSource.clip = backgroundMusic;
                break;
        }

        audioSource.Play();
    }

    public void SetMusic(int index)
    {
        switch(index)
        {
            case 0:
                audioSource.clip = titleMusic;
                break;
            case 1:
                audioSource.clip = backgroundMusic;
                break;
            case 2:
                audioSource.clip = enemyMusic;
                break;
            case 3:
                audioSource.clip = bossMusic;
                break;
            case 4:
                audioSource.clip = backgroundMusic;
                break;
        }
        StartCoroutine(FadeAudioSource.StartFade(audioSource, 2f, 0.5f));
        audioSource.Play();
    }

    public AudioSource GetAudioSource()
    {
        return audioSource;
    }



}


