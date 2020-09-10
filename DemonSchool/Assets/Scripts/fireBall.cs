using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class fireBall : MonoBehaviour
{
    public float speed;//Setting up Fireball's fly speed(can be reset in Unity's UI)
    public Rigidbody2D rb;//Setting up Fireball's Rigidbody

    void Start()
    {
        rb.velocity = transform.right * speed;//fireball fly status
    }



}
