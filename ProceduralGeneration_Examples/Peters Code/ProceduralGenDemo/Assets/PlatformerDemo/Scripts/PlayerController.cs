using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public class GroundState
    {
        private GameObject player;
        private float width;
        private float height;
        private float length;

        private Collider2D playerCollider;

        // GroundState constructor.  Sets offsets for raycasting.
        public GroundState(GameObject playerRef)
        {
            player = playerRef;
            playerCollider = player.GetComponent<Collider2D>();
            width = playerCollider.bounds.extents.x + 0.1f;
            height = playerCollider.bounds.extents.y + 0.2f;
            length = 0.05f;
        }

        // Returns whether or not player is touching wall.
        public bool isWall()
        {
            bool left = Physics2D.Raycast(new Vector2(player.transform.position.x - width, 
                                                      player.transform.position.y), 
                                                      -Vector2.right, length);
            bool right = Physics2D.Raycast(new Vector2(player.transform.position.x + width, 
                                                       player.transform.position.y), 
                                                       Vector2.right, length);

            return left || right;
        }

        // Returns whether or not player is touching ground.
        public bool isGround()
        {
            // Directly down
            return Physics2D.Raycast(new Vector2(player.transform.position.x, 
                                                 player.transform.position.y - height),
                                                 -Vector2.up, length);
        }

        // Returns whether or not player is touching wall or ground.
        public bool isTouching()
        {
            return isGround() || isWall();
        }

        //Returns direction of wall.
        public int wallDirection()
        {
            // If there is a wall to the left.
            if (Physics2D.Raycast(new Vector2(player.transform.position.x - width,
                                                      player.transform.position.y),
                                                      -Vector2.right, length))
                return -1;

            // If there is a wall to the right.
            if (Physics2D.Raycast(new Vector2(player.transform.position.x + width,
                                                      player.transform.position.y),
                                                      Vector2.right, length))
                return 1;

            // No wall to either side.
            return 0;
        }
    }

    // Character movement speed.
    public float speed = 14f;
    // Horizontal acceleration when not in the air.
    public float accel = 6f;
    // Horizontal acceleration when in the air.
    public float airAccel = 3f;
    // Vertical speed when jumping.
    public float jump = 14f;  

    private GroundState groundState;
    private Rigidbody2D rigidBody;

    void Start()
    {
        //Create an object to check if player is grounded or touching wall
        groundState = new GroundState(transform.gameObject);
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private Vector2 input;

    void Update()
    {
        // Handle input
        if (Input.GetKey(KeyCode.LeftArrow))
            input.x = -1;
        else if (Input.GetKey(KeyCode.RightArrow))
            input.x = 1;
        else
            input.x = 0;

        if (Input.GetKeyDown(KeyCode.Space))
            input.y = 1;
    }

    void FixedUpdate()
    {
        // Move player with force to the left/right
        rigidBody.AddForce(new Vector2(
            ((input.x * speed) - rigidBody.velocity.x)  
            * (groundState.isGround() ? accel : airAccel) // different acceleration in air
            , 0)); // No vertical force

        // Stop player if input.x is 0 (and grounded) and jump if input.y is 1
        rigidBody.velocity = new Vector2(
            (input.x == 0 && groundState.isGround()) ? 0 : rigidBody.velocity.x, 
            (input.y == 1 && groundState.isTouching()) ? jump : rigidBody.velocity.y);

        // Add force negative to wall direction (with speed reduction)
        if (groundState.isWall() && !groundState.isGround() && input.y == 1)
            rigidBody.velocity = new Vector2(-groundState.wallDirection() * speed * 0.75f, 
                                             rigidBody.velocity.y); 

        // Only apply the jumps for single button down presses.
        input.y = 0;
    }
}