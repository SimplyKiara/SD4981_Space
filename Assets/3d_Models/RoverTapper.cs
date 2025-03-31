using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;
using System;

public class RoverTapper : MonoBehaviour
{
    private PressGesture pressGesture;
    private bool isPressed;

    private void OnEnable()
    {
        pressGesture = GetComponent<PressGesture>();

        pressGesture.Pressed += pressedHandler;
    }

    void Update()
    {
        
    }

    private void pressedHandler(object sender, EventArgs e)
    {
        Debug.Log("Rover tapped");
        if (isPressed) return;
        isPressed = true;
        Invoke("ResetPress", 0.1f);

        
        GameManager.instance.ChangeCollectedWater(2f);
        Destroy(gameObject);
    }

    private void ResetPress()
    {
        isPressed = false;
    }
}
