using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class fireBall : MonoBehaviour
{
    public float speed;//Setting up Fireball's fly speed(can be reset in Unity's UI)
    public Rigidbody2D rb;//Setting up Fireball's Rigidbody
    public Sprite touch;


    void Start()
    {
        rb.velocity = transform.right * speed;//fireball fly status

        Destroy(gameObject, 1);
    }

    private void Update()
    {
        this.transform.Rotate(1, 1, speed);
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.tag == "enemy")
        {
            this.GetComponent<SpriteRenderer>().sprite = touch;
            Destroy(gameObject,0.05f);
        }

        else if(hit.tag == "boss")
        {
            this.GetComponent<SpriteRenderer>().sprite = touch;
            Destroy(gameObject, 0.05f);
        }

        else if(hit.tag == "map" || hit.tag == "horoWall" || hit.tag == "door" || hit.tag == "vertWall")
        {
            this.GetComponent<SpriteRenderer>().sprite = touch;
            Destroy(gameObject, 0.05f);
        }
      
            
        
    }



}
