using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class RoverController : MonoBehaviour
{
    public float speed = 5f;
    public Text posText;

    Vector2 lastClickedPos;
    bool moving;
    bool atGoal;

    private Vector2 startPos;
    private float currentPosX;
    private float currentPosY;
    private Rigidbody2D rb;

    private Vector3 spot;

    Vector2 direction;
    Quaternion rotation;
    Quaternion rotation2;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentPosX = transform.position.x * 100;
        currentPosY = transform.position.y * 100;
        startPos = transform.position;
        atGoal = false;
    }

    private void Update()
    {
        // Displayed coordinates
        currentPosX = transform.position.x * 100;
        currentPosY = transform.position.y * 100;

        // Moving direction and angle
        if (!atGoal && Input.GetMouseButtonDown(0))
        {
            lastClickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            moving = true;

            direction = (lastClickedPos - (Vector2)transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Get angle in degrees
            transform.rotation = Quaternion.Euler(0, 0, angle - 90); // Adjust for object facing
        }

        // Stop at last touched position
        if (moving && (Vector2)transform.position != lastClickedPos)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, lastClickedPos, step);
        }

        posText.text = $"Your position:\nx = {currentPosX:F0}, y = {currentPosY:F0}";
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "2D Danger")
        {
            Debug.Log("Collided with danger");
            rb.velocity = Vector2.zero; 
            transform.position = startPos;
            rotation2.z = 0;
            rotation2.w = 0;
            transform.rotation = rotation2;
            moving = false; 
        }

        if (collision.gameObject.tag == "2D Goal")
        {
            Debug.Log("Reached goal");
            atGoal = true;
            posText.text = "Goal reached!";
            StartCoroutine(WaitCoroutine());
            SceneManager.LoadScene("IceMining_P2");
        }
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(3);
    }
}
