using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDungeon : Singleton<TutorialDungeon>
{

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
