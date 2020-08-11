using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBehaviour : MonoBehaviour
{
    [Tooltip("The force which is added when the player jumps")]
    public Vector2 jumpForce = new Vector2(0, 300);
    private bool beenHit;
    private Rigidbody2D rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        beenHit = false;
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // LateUpdate is called every frame
    void LateUpdate()
    {
        if ((Input.GetKeyUp("space") || Input.GetMouseButtonDown(0)) && !beenHit)
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.AddForce(jumpForce);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        beenHit = true;
        GameController.speedModifier = 0;
        GetComponent<Animator>().speed = 0.0f;
    }
}
