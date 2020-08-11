using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [HideInInspector] public static float speedModifier;


    // Start is called before the first frame update
    void Start()
    {
        speedModifier = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
