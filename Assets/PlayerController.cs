using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControler : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb;
    private Collider2D coll;
    private AudioSource footstep;
    private int totalPigeons;
    private int totalFalcons;
    [SerializeField] private LayerMask Ground;
    [SerializeField] private int pigeons = 0;
    [SerializeField] private TextMeshProUGUI pigeonsText;
    [SerializeField] private TextMeshProUGUI falconText; 
    private int destroyedFalcons = 0;  // Number of enemies destroyed
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpforce = 10f;
    [SerializeField] private float hurtforce = 10f;
    [SerializeField] private int health = 4; // Give the player some health
    [SerializeField] private TextMeshProUGUI healthAmount;
    [SerializeField] private float countdownTime = 60f; // 60 seconds timer
    [SerializeField] private TextMeshProUGUI countdownText; // UI text for timer
    [SerializeField] private TextMeshProUGUI levelMessage;
    private bool isTimeUp = false; // Track if time has expired

    private State state;
    public enum State
    {
        falling,
        idle,
        jumping,
        run,
        hurt

    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        healthAmount.text = health.ToString(); // Display correct starting health
        footstep = GetComponent<AudioSource>();
        totalPigeons = GameObject.FindGameObjectsWithTag("Collectable").Length; // Count all collectibles in the level
        totalFalcons = GameObject.FindGameObjectsWithTag("Enemy").Length;

    }

    // Update is called once per frame
    void Update()
    {
        if (state != State.hurt)
        {
            Movement();
        }

        if (!isTimeUp) // Only run the timer if time is not up
        {
            UpdateTimer();
        }

        if (state != State.hurt) // Keep movement working
        {
            Movement();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);  // Log the name of the object collided with

        // Collecting collectibles
        if (collision.tag == "Collectable")
        {
            Destroy(collision.gameObject);
            pigeons += 1;
            pigeonsText.text = pigeons.ToString();
        }

        // Collecting powerups
        if (collision.tag == "Powerup")
        {
            Destroy(collision.gameObject);
            jumpforce = 30f;
            GetComponent<SpriteRenderer>().color = Color.yellow;
            StartCoroutine(ResetPower());
        }

        // Checking if player touches the finish area and if all pigeons are collected
        if (collision.gameObject.name == "Finish")
        {
            if (pigeons >= totalPigeons)
            {
                levelMessage.text = "Level Complete! Entering next level...";
                StartCoroutine(LoadNextLevel());
            }
            else
            {
                levelMessage.text = "You need to save all the pigeons!";
                StartCoroutine(ClearMessage());
            }
        }

        // Checking if player touches the FinishGame asset and if all pigeons are collected
        // Checking if player touches the FinishGame asset and if all pigeons are collected and all falcons are destroyed
        if (collision.gameObject.CompareTag("FinishGame"))
        {
            // Check both pigeons collected and falcons destroyed
            if (pigeons >= totalPigeons && destroyedFalcons == totalFalcons)
            {
                levelMessage.text = "All pigeons are saved! Thanks for playing Rule of The Pigeons!";
                StartCoroutine(EndGame());  // Start the game over coroutine
            }
            else
            {
                // If either pigeons or falcons aren't fully collected/destroyed
                if (pigeons < totalPigeons)
                {
                    levelMessage.text = "You need to save all the pigeons!";
                }
                else if (destroyedFalcons < totalFalcons)
                {
                    levelMessage.text = "You need to destroy all the falcons!";
                }
                StartCoroutine(ClearMessage());
            }
        }

    }


    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(5);  // Show the "Game Over" message for 5 seconds                                         
        Application.Quit();  // Close the game if you want to exit
    }


    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(2); // Show message for 2 seconds
        SceneManager.LoadScene("LevelTwo"); // Change to your next level name
    }

    private IEnumerator ClearMessage()
    {
        yield return new WaitForSeconds(2); // Message stays for 2 seconds
        levelMessage.text = ""; // Clear message
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Debug.Log("Player collided with enemy!");

            // Check if player is falling (i.e., colliding from above)
            if (rb.velocity.y < 0) // Player is falling down (negative Y velocity)
            {
                Destroy(other.gameObject); // Destroy the enemy if player falls on top
                destroyedFalcons++;  // Increment destroyed enemies count
                falconText.text = destroyedFalcons.ToString(); // Update the UI for falcon count
            }
            else
            {
                // If player is not falling, reduce health and apply damage (no destruction of the enemy)
                state = State.hurt;
                HandleHeath(); // Call health reduction

                // Apply knockback based on enemy position
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    rb.velocity = new Vector2(-hurtforce, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(hurtforce, rb.velocity.y);
                }
            }
        }
    }



    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");

        if (hDirection > 0) // Moving right
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1); // Face right
        }
        else if (hDirection < 0) // Moving left
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1); // Face left (flip horizontally)
        }

        if (hDirection != 0) // Player is moving left or right
        {
            rb.velocity = new Vector2(hDirection * speed, rb.velocity.y); // Use hDirection to scale movement
            transform.localScale = new Vector2(hDirection > 0 ? 1 : -1, 1); // Flip sprite based on direction
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // Stop horizontal movement when no input
        }

        if (Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        }

    }

    private void HandleHeath()
    {
        health -= 1; // Reduce health
        healthAmount.text = health.ToString(); // Update UI

        if (health <= 0) // If health reaches 0 or below
        {
            levelMessage.text = "All lives are gone! Level is restarting..."; // Display message
            StartCoroutine(RestartAfterDelay()); // Restart the level after a delay
        }
        else
        {
            StartCoroutine(RecoverFromHurt()); // Recover after a short delay
        }
    }

    private IEnumerator RecoverFromHurt()
    {
        yield return new WaitForSeconds(0.5f); // Wait 0.5 seconds
        state = State.idle; // Reset state so the player can move again
    }

    private IEnumerator ResetPower()
    { 
    yield return new WaitForSeconds(5);
    GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void FootStep()
    {
        footstep.Play();
    }

    private void UpdateTimer()
    {
        if (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime; // Reduce time
            countdownText.text = Mathf.Ceil(countdownTime).ToString(); // Display as whole number
        }
        else
        {
            isTimeUp = true;
            RestartLevel(); // Call function to restart
        }
    }

    private void RestartLevel()
    {
        levelMessage.text = "Time's up! Restarting level...";
        Debug.Log("Time is up! Restarting level...");
        StartCoroutine(RestartAfterDelay());
    }

    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(2); // Show message for 2 seconds
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }



}


