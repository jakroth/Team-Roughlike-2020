using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    public Text playerFinalScore;
    public Text playerFinalLevel;

    // Start is called before the first frame update
    void Start()
    {
        playerFinalScore.text = PlayerStats.score.ToString();
        playerFinalLevel.text = PlayerStats.level.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
