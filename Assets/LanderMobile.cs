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
    public float drag = 0.98f;  
    private Rigidbody rb;
    public float thrustForce = 40f;
    public float jumpForce = 0.000000001f;
    public bool surfaceTouched = false;
    public Camera camera1;
    public Camera camera2;
    public GameObject EndPanel;

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

    void FixedUpdate()
    {
        ApplyDrag();
    }

    public void MoveLeft()
    {
        Vector3 force = -transform.right * thrustForce; 
        rb.AddForce(force, ForceMode.Acceleration);

    }

    public void MoveRight()
    {
        Vector3 force = transform.right * thrustForce; 
        rb.AddForce(force, ForceMode.Acceleration);
    }

    public void MoveUp() 
    {
        Vector3 force = -transform.up * thrustForce; 
        rb.AddForce(force, ForceMode.Acceleration);
    }

    public void MoveDown() 
    {
        Vector3 force = transform.up * thrustForce; 
        rb.AddForce(force, ForceMode.Acceleration);
    }

    public void Thrust() {
        Debug.Log("jump pressed");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ApplyDrag()
    {
        // Reduce the velocity gradually to simulate resistance
        rb.velocity *= drag;
    }


    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "floor" | col.gameObject.tag == "LandingPadWorst" | col.gameObject.tag == "LandingPadMid" | col.gameObject.tag == "LandingPadBest") 
        {
            surfaceTouched = true;
            Debug.Log("floor or landing pad touched");
            EndPanel.SetActive(true);
        }
    }

}
