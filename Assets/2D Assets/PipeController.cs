using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeController : MonoBehaviour
{
    private bool moving;
    private float startPosX;
    private float startPosY;
    private Vector3 resetPosition;
    private float moveTimeout = 1.0f; // Timeout duration in seconds
    private Coroutine moveCoroutine;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        gameObject.tag = "Pipette";
        animator.SetBool("Filled", false);

        resetPosition = this.transform.localPosition;
    }

    void Update()
    {
        // Handle touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));

            if (touch.phase == TouchPhase.Began)
            {
                startPosX = worldPos.x - transform.localPosition.x;
                startPosY = worldPos.y - transform.localPosition.y;
                moving = true;
            }
            else if (touch.phase == TouchPhase.Moved && moving)
            {
                transform.localPosition = new Vector3(worldPos.x - startPosX, worldPos.y - startPosY, transform.localPosition.z);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                moving = false;
                CheckCollisions();
                transform.localPosition = resetPosition;
            }
        }

        // Handle mouse input
        if (moving && Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.localPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, transform.localPosition.z);
        }
    }

    void PipeHandler()
    {
        int layerMask = ~LayerMask.GetMask("Tools");
        Collider2D collider = Physics2D.OverlapPoint(transform.position, layerMask);

        if (collider != null && collider.gameObject != gameObject)
        {
            Debug.Log("Detected collider: " + collider.name + " with tag: " + collider.tag);
            if (collider.CompareTag("Water"))
            {
                ChangeTag();
            }
            else if (collider.CompareTag("Veggies"))
            {
                if (gameObject.tag == "WaterPipette")
                {
                    ChangeTag();
                    Debug.Log("Watered!");
                }
                else
                {
                    Debug.Log("Cannot water!");
                }
            }
            else
            {
                Debug.Log($"Action not identified: {gameObject.tag} to {collider.tag}");
            }
        }
    }

    void ChangeTag()
    {
        if (gameObject.tag == "WaterPipette")
        {
            animator.SetBool("Filled", false);
            gameObject.tag = "Pipette";
        }
        else if (gameObject.tag == "Pipette")
        {
            animator.SetBool("Filled", true);
            gameObject.tag = "WaterPipette";
        }
        animator.Update(0);
    }

    private void OnMouseDown()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        startPosX = mousePos.x - transform.localPosition.x;
        startPosY = mousePos.y - transform.localPosition.y;
        moving = true;
    }

    private void OnMouseUp()
    {
        moving = false;
        CheckCollisions();
        transform.localPosition = resetPosition;
    }

    private void CheckCollisions()
    {
        int layerMask = ~LayerMask.GetMask("Tools");
        Collider2D collider = Physics2D.OverlapPoint(transform.position, layerMask);

        if (collider != null && collider.gameObject != gameObject)
        {
            if ((gameObject.tag == "WaterPipette") && collider.CompareTag("Veggies"))
            {
                collider.gameObject.SendMessage("VeggiesAction");
            }
            else if (gameObject.tag == "Pipette" && collider.CompareTag("Water"))
            {
                PipeHandler();
            }
            else if (gameObject.tag == "WaterPipette" && collider.CompareTag("Pots"))
            {
                collider.gameObject.SendMessage("PotAction");
            }
            else
            {
                Debug.Log("MoveSystem cannot identify action, tag = " + collider.tag);
            }
        }
        else
        {
            Debug.Log("MoveSystem cannot identify collision");
        }
    }
}
