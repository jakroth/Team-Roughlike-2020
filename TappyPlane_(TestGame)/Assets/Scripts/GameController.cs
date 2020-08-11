using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [HideInInspector] public static float speedModifier;

    [Header("Obstacle Information")]

    [Tooltip("The Obstacle that we will spawn")]
    public GameObject obstacleReference;

    [Tooltip("Minimum Y value used for obstacle")]
    public float obstacleMinY = -1.3f;

    [Tooltip("Maximum Y value used for obstacle")]
    public float obstacleMaxY = 1.3f;

    private static Text scoreText;
    private static int score;

    // Start is called before the first frame update
    void Start()
    {
        speedModifier = 1.0f;
        gameObject.AddComponent<GameStartBehaviour>();
        score = 0;
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateObstacle()
    {
        Instantiate(obstacleReference, 
            new Vector3(RepeatingBackground.ScrollWidth,
            Random.Range(obstacleMinY, obstacleMaxY), 0.0f),
            Quaternion.identity);
    }

    public static int Score
    {
        get 
        { 
            return score; 
        }
        set
        {
            score = value;
            scoreText.text = score.ToString();
        }
    }

}
