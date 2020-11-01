using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallAnimation : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0f, -1f * Time.deltaTime, 0f);
    }
}
