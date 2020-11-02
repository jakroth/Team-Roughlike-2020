using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadNextScene()
    {
        StartCoroutine(FadeAudioSource.StartFade(MusicController.instance.GetAudioSource(), 2f, 0f));
        StartCoroutine(NextScene());
    }

    private IEnumerator NextScene()
    {
        yield return new WaitForSeconds(2f);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        GameController.instance.UpdatePauseState(false);
        MusicController.instance.SetMusic(currentSceneIndex + 1);
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
    public void LoadStartScene()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadScene(int a)
    {
        SceneManager.LoadScene(a);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
