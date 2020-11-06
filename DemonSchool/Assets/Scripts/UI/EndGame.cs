using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public Text playerFinalScore;
    public Text playerFinalLevel;
    public Text playerFinalStudentsRescued;

    public GameObject studentPrefab;
    public PhysicsMaterial2D bounceMat;

    // Start is called before the first frame update
    void Start()
    {
        playerFinalScore.text = PlayerStats.score.ToString();
        playerFinalLevel.text = PlayerStats.level.ToString();

        if(SceneManager.GetActiveScene().buildIndex == 4)
        {
            playerFinalStudentsRescued.text = PlayerStats.students.ToString();

            for(int i = 0; i < PlayerStats.students; i++)
            {
                GameObject go = Instantiate(studentPrefab, new Vector3(Random.Range(-8f, 8f), -3f, 0), Quaternion.identity);
                go.tag = Random.Range(0, 2) == 0 ? "boyStudent" : "girlStudent";
                go.AddComponent<Rigidbody2D>();
                go.GetComponent<BoxCollider2D>().isTrigger = false;
                go.GetComponent<BoxCollider2D>().sharedMaterial = bounceMat;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
