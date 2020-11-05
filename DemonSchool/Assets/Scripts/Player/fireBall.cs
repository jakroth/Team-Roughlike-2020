using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class fireBall : MonoBehaviour
{
    public float speed;//Setting up Fireball's fly speed(can be reset in Unity's UI)
    public Rigidbody2D rb;//Setting up Fireball's Rigidbody
    public Sprite touch;

    [SerializeField] private AudioClip attackSound = null;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(attackSound);
        rb.velocity = transform.right * speed;//fireball fly status
        Destroy(gameObject, 0.5f);
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
            Destroy(gameObject,0.025f);
        }

        else if(hit.tag == "boss")
        {
            this.GetComponent<SpriteRenderer>().sprite = touch;
            Destroy(gameObject, 0.025f);
        }

        else if(hit.tag == "map" || hit.tag == "horoWall" || hit.tag == "door" || hit.tag == "finalDoor" || hit.tag == "vertWall" || hit.tag == "vertNoneAbove")
        {
            this.GetComponent<SpriteRenderer>().sprite = touch;
            Destroy(gameObject, 0.025f);
        }
      
            
        
    }



}
