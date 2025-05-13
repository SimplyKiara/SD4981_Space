using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// using static UnityEditor.Timeline.TimelinePlaybackControls;
// using static UnityEngine.RuleTile.TilingRuleOutput;

public class RoverController : MonoBehaviour
{
    public float speed = 5f;
    public Slider progressBar;
    public Text taskText;
    public GameObject endPanel;

    private Rigidbody2D rb;
    private InputAction moveAction;
    private PlayerInput playerInput;

    private Vector2 startPos;
    private Vector2 movementInput;
    private Quaternion startRotation;

    private float fillSpeed = 0.15f;
    private float drainSpeed = 0.5f;
    private bool isColliding = false;
    private bool wasFullyFilled = false;
    private GameObject collidedObject;

    private int tasksDone = 0;
    private bool done = false;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move", true);

        if (moveAction == null)
        {
            Debug.LogError("Move action not found");
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position; // Stores the initial position of the rover
        startRotation = transform.rotation; // Stores original rotation

        progressBar.gameObject.SetActive(false);
        endPanel.SetActive(false);
        taskText.text = "Spot(s) found: " + tasksDone;
    }

    private void Update()
    {
        if (isColliding && progressBar.value < progressBar.maxValue)
        {
            progressBar.value += fillSpeed * Time.deltaTime;

            // Check if progress bar is fully filled
            if (progressBar.value >= progressBar.maxValue)
            {
                Debug.Log("One Progress Done");

                // Change the tag of the collided object
                if (collidedObject != null)
                {
                    collidedObject.tag = "Untagged";
                    progressBar.value = 0;
                    progressBar.gameObject.SetActive(false);
                }

                wasFullyFilled = true;
                tasksDone++;
            }
        }
        else if (!isColliding && progressBar.value > 0f)
        {
            progressBar.value -= drainSpeed * Time.deltaTime;
        }

        // Hide the progress bar when it fully drains
        if (progressBar.value <= 0f)
        {
            progressBar.gameObject.SetActive(false);
            wasFullyFilled = false; // Reset flag
        }

        taskText.text = "Spot(s) found: " + tasksDone;

        if ((tasksDone == 2) && !done)
        {
            done = true;
            callPanel();
        }
    }

    private void OnEnable() => moveAction?.Enable();
    private void OnDisable() => moveAction?.Disable();

    private void FixedUpdate()
    {
        movementInput = moveAction.ReadValue<Vector2>();

        if (movementInput.magnitude > 0)
        {
            // Ensure movement applies both X and Y values
            Vector2 direction = new Vector2(movementInput.x, movementInput.y).normalized;

            // Apply movement in all directions
            rb.velocity = direction * speed;

            // Rotate rover to face movement direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "2D Danger")
        {
            Debug.Log("Collided with danger");
            rb.velocity = Vector2.zero; transform.position = startPos;
            transform.rotation = Quaternion.identity;
        }

        if (collision.gameObject.tag == "2D Goal")
        {
            Debug.Log("Reached a site");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "2D Goal")
        {
            isColliding = true;
            progressBar.gameObject.SetActive(true); // Show when entering the colliding zone
            collidedObject = collision.gameObject; // Store reference to collided object
        }
        else
        {
            isColliding = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "2D Goal")
        {
            isColliding = false;
        }
    }

    private void callPanel()
    {
        endPanel.SetActive(true);
    }
}
