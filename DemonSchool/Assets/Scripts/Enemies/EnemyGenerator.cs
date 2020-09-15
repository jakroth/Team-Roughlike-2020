using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{


    // 2D array of enemies (made from the enemyPrefab)
    public GameObject[,] enemyMap;

    // the prefab for all the Enemies, set in the Inspector
    public GameObject enemyPrefab;
}
