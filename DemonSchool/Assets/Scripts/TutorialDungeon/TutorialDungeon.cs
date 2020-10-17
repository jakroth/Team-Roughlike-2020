using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDungeon : Singleton<TutorialDungeon>
{
    // the prefab for all the Tiles, set in the Inspector
    public GameObject tilePrefab;

    [Header("Hell Sprites")]
    // tile set for the Hell Tiles
    public List<Sprite> hellTiles;

    [Header("Normal Sprites")]
    // tile set for the Normal Tiles
    public List<Sprite> normalTiles;


    // Start is called before the first frame update
    void Start()
    {
        // put camera on player
        Camera.main.GetComponent<FollowCameraBehaviour>().setMap(new Vector2(30, 20), new Vector2(1,1));

    }


    // Update is called once per frame
    void Update()
    {
 

    }


}
