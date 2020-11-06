using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static bool isPaused = false;
    public static bool IsPaused { get => isPaused; set => isPaused = value; }

    public static GameController instance = null;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this);
        }
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    public void UpdatePauseState(bool pauseValue)
    {
        isPaused = pauseValue;
    }

    public bool GetPauseState()
    {
        return isPaused;
    }
}
