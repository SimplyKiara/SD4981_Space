using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudController : MonoBehaviour
{
    private bool moving;
    private float startPosX;
    private float startPosY;
    private Vector3 resetPosition;

    private float moveTimeout = 1.0f; // Timeout duration in seconds
    private Coroutine moveCoroutine;

    void Start()
    {
        resetPosition = this.transform.localPosition;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartMovement(worldPos);
                    break;
                case TouchPhase.Moved:
                    if (moving)
                        MoveObject(worldPos);
                    break;
                case TouchPhase.Ended:
                    StopMovement();
                    break;
            }
        }

        if (moving && Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MoveObject(mousePos);
        }
    }

    private void HandleInput()
    {
        
    }

    private void OnMouseDown()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        StartMovement(mousePos);

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(StopMovementAfterTimeout());
    }

    private void OnMouseUp()
    {
        StopMovement();
    }

    private void StartMovement(Vector3 inputPos)
    {
        startPosX = inputPos.x - transform.localPosition.x;
        startPosY = inputPos.y - transform.localPosition.y;
        moving = true;
    }

    private void MoveObject(Vector3 inputPos)
    {
        transform.localPosition = new Vector3(inputPos.x - startPosX, inputPos.y - startPosY, transform.localPosition.z);
    }

    private void StopMovement()
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
            if (collider.CompareTag("Pots"))
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

    // Timeout Coroutine
    private IEnumerator StopMovementAfterTimeout()
    {
        yield return new WaitForSeconds(moveTimeout);
        moving = false;
        transform.localPosition = resetPosition;
    }
}
