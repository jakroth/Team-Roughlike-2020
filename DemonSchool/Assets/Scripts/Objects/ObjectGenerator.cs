using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{

    // 2D array of objects (made from the objectPrefab)
    public GameObject[,] objectMap;

    // the prefab for all the Object, set in the Inspector
    public GameObject objectPrefab;

}
