using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrillController : MonoBehaviour
{
    public Button myButton;
    public Text iceText;

    private Animator animator;
    private Rigidbody2D rb;

    public float speed = 2f;
    private Vector3 startPos;
    private Vector3 spot;
    private Vector3 moveDirection;
    bool suitableAngle;
    Vector2 direction;
    Quaternion rotation;
    Quaternion rotation2;

    private bool Status = false;
    private int collectedIce = 0;
    bool atGoal = false;
    bool moving;
    int totalIce;

    public void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        myButton.onClick.AddListener(TaskOnClick);
        startPos = transform.position;

        GameObject[] gos = GameObject.FindGameObjectsWithTag("2D Goal");
        totalIce = gos.Length;
    }

    public void Update()
    {
        if (!atGoal)
        {
            if (Input.GetMouseButton(0))
            {
                spot = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                spot.z = 0;
            }

            if (Input.GetMouseButtonUp(0) && suitableAngle)
            {
                Debug.Log("Mouse released");
                Vector3 mouseReleasePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseReleasePos.z = 0; // Ensure it's in 2D space

                // Calculate the movement direction
                moveDirection = (mouseReleasePos - transform.position).normalized; // Normalized vector
                moving = true;
            }
        }

        if (Time.timeScale != 0)
        {
            direction = spot - transform.position;
            rotation = Quaternion.LookRotation(direction, Vector3.forward);
            rotation2.x = 0;
            rotation2.y = 0;

            if ((Math.Abs(rotation.y) < 0.49))
            {
                rotation2.z = rotation.z;
                rotation2.w = rotation.w;
                transform.rotation = rotation2;
                suitableAngle = true;
            }
            else
            {
                suitableAngle = false;
            }
        }

        if (moving)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, spot, step);
        }
        else
        {
            transform.position = startPos;
        }

        iceText.text = $"Ice collected:\n{collectedIce}/{totalIce}";
    }

    void TaskOnClick()
    {
        Status = !Status;
        // false = Yellow/Break, true = Blue/Collect
        animator.SetBool("Status", Status);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided with danger");
        if (collision.gameObject.tag == "2D Danger")
        {
            if (!Status)
            {
                Destroy(collision.gameObject);
                Debug.Log("Destroyed rock");
            }
            else
            {
                Debug.Log("Incorrect type of drill");
            }
        }

        if (collision.gameObject.tag == "2D Goal")
        {
            if (Status)
            {
                Destroy(collision.gameObject);
                Debug.Log("Collected ice");
                collectedIce += 1;
                if (collectedIce == 3)
                {
                    atGoal = true;
                }
            }
            else
            {
                Debug.Log("Incorrect type of drill");
            }
        }

        rb.velocity = Vector2.zero;
        rotation2.z = 0;
        rotation2.w = 0;
        transform.rotation = rotation2;
        moving = false;
    }
}