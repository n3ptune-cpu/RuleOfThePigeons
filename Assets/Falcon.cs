using UnityEngine;

public class Falcon : MonoBehaviour
{
    public bool facingLeft = true;
    public float movementSpeed = 5f;  // Horizontal speed of the enemy
    public float jumpHeight = 10f;    // Vertical jump height (if needed)
    public float leftCap = -5f;       // Left boundary
    public float rightCap = 5f;       // Right boundary
    public LayerMask Ground;          // Ground layer for detecting if the enemy is grounded

    private Rigidbody2D rb;
    private Collider2D coil;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coil = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        // Enemy movement logic in FixedUpdate (for physics-based movement)
        MoveEnemy();
    }

    void MoveEnemy()
    {
        // Check if the enemy is grounded to allow movement
        bool isGrounded = coil.IsTouchingLayers(Ground);

        if (facingLeft)
        {
            if (transform.position.x > leftCap)
            {
                // Ensure the enemy is facing left
                if (transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }

                // Move enemy left if grounded
                if (isGrounded)
                {
                    rb.velocity = new Vector2(-movementSpeed, rb.velocity.y);  // Move horizontally left
                }
            }
            else
            {
                facingLeft = false; // Switch direction to right
            }
        }
        else
        {
            if (transform.position.x < rightCap)
            {
                // Ensure the enemy is facing right
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }

                // Move enemy right if grounded
                if (isGrounded)
                {
                    rb.velocity = new Vector2(movementSpeed, rb.velocity.y);  // Move horizontally right
                }
            }
            else
            {
                facingLeft = true; // Switch direction to left
            }
        }
    }
}
