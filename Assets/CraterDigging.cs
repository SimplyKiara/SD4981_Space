using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TouchScript.Gestures;
using System;

public class CraterDigging : MonoBehaviour
{
    private LongPressGesture longPressGesture;
    private int craterHP = 350;
    private bool holding = false;

    private void Update()
    {
        if (craterHP <= 0)
        {
            Destroy(gameObject);
        }

        if (holding)
        {
            Invoke("changeHP", 1.0f);
        }
    }

    private void OnEnable()
    {
        longPressGesture = GetComponent<LongPressGesture>();

        longPressGesture.StateChanged += longPressedHandler;
    }

    private void OnDisable()
    {
        longPressGesture.StateChanged -= longPressedHandler;
    }

    private void longPressedHandler(object sender, GestureStateChangeEventArgs e)
    {
        if (e.State == Gesture.GestureState.Recognized)
        {
            holding = true;
        }
        else if (e.State == Gesture.GestureState.Failed)
        {
            holding = false;
        }
    }

    private void changeHP()
    {
        craterHP -= 10;
    } 
}
