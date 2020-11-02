using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static bool isPaused;
    public GameObject pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("escape"))
        {
            isPaused = true;
            Time.timeScale = (isPaused ? 0 : 1);
            pauseMenu.SetActive(isPaused);
        }   
    }

    public void resumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;

    }

}
