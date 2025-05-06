using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;

public class LongPressDestroy : MonoBehaviour
{
    private LongPressGesture longPressGesture;

    private void Start()
    {
        // Get the LongPressGesture component attached to this GameObject
        longPressGesture = GetComponent<LongPressGesture>();

        if (longPressGesture != null)
        {
            longPressGesture.StateChanged += longPressedHandler;
            Debug.Log($"Subscribed to {gameObject.name}'s LongPressGesture");
        }
        else
        {
            Debug.LogError("LongPressGesture component is missing!");
        }
    }

    private void longPressedHandler(object sender, GestureStateChangeEventArgs e)
    {
        if (e.State == Gesture.GestureState.Recognized)
        {
            Debug.Log("Long press detected on coveringCrater!");
            Destroy(gameObject);
            Debug.Log("coveringCrater destroyed.");
        }
    }
}
