using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanderMobile : MonoBehaviour
{
    public Button leftButton;
    public Button rightButton;
    public Button UpButton;
    public Button DownButton;
    public Button ThrustButton;
    public float moveSpeed = 10f;
    public float rotationSpeed = 10f;
    private Rigidbody rb;
    //private TaskManager taskManager;

    public float jumpForce = 0.000000001f;
    public bool surfaceTouched = false;

    public Camera camera1;
    public Camera camera2;

    private void Start()
    {   
        //taskManager = FindObjectOfType<TaskManager>();
        rb = GetComponent<Rigidbody>();
        ThrustButton.onClick.AddListener(Thrust);
        leftButton.onClick.AddListener(MoveLeft);
        rightButton.onClick.AddListener(MoveRight);
        UpButton.onClick.AddListener(MoveUp);
        DownButton.onClick.AddListener(MoveDown);
        camera1.enabled = true;
        camera2.enabled = false;
        gameObject.SetActive(false);
    }

    void Update() {
        Physics.gravity = new Vector3(0, -2.0f, 0);
        if (surfaceTouched == true) {
            camera1.enabled = false;
            camera2.enabled = true;
            leftButton.enabled = false;
            rightButton.enabled = false;
            ThrustButton.enabled = false;
            UpButton.enabled = false;
            DownButton.enabled = false;
        }   
    }

    public void MoveLeft()
    {
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        //transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
    }

    public void MoveRight()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        //transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void MoveUp() 
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
    }

    public void MoveDown() 
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }

    public void Thrust() {
        Debug.Log("jump pressed");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "floor" | col.gameObject.tag == "LandingPadWorst" | col.gameObject.tag == "LandingPadMid" | col.gameObject.tag == "LandingPadBest") 
        {
            surfaceTouched = true;
            Debug.Log("floor or landing pad touched");
        }
    }

}
