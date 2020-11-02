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
    public void LoadStartScene()
    {
        StartCoroutine(FadeAudioSource.StartFade(MusicController.instance.GetAudioSource(), 2f, 0f));
        StartCoroutine(StartScene());
    }
    public void LoadScene(int a)
    {
        StartCoroutine(FadeAudioSource.StartFade(MusicController.instance.GetAudioSource(), 2f, 0f));
        StartCoroutine(SelectScene(a));
    }
    public void QuickStart()
    {
        GameController.instance.UpdatePauseState(false);
        MusicController.instance.SetMusic(0);
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
# if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    private IEnumerator NextScene()
    {
        yield return new WaitForSeconds(2f);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        GameController.instance.UpdatePauseState(false);
        MusicController.instance.SetMusic(currentSceneIndex + 1);
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
    private IEnumerator StartScene()
    {
        yield return new WaitForSeconds(2f);
        GameController.instance.UpdatePauseState(false);
        MusicController.instance.SetMusic(0);
        SceneManager.LoadScene(0);
    }
    private IEnumerator SelectScene(int a)
    {
        yield return new WaitForSeconds(2f);
        GameController.instance.UpdatePauseState(false);
        MusicController.instance.SetMusic(a);
        SceneManager.LoadScene(a);
    }

}
