using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;

public class GHouseController : MonoBehaviour
{
    public GameObject popUpPanel; // Assign the UI popup

    private void Start()
    {
        popUpPanel.SetActive(false);

        // Find all child objects with LongPressGesture
        foreach (Transform child in transform)
        {
            LongPressGesture longPressGesture = child.GetComponent<LongPressGesture>();

            if (longPressGesture != null)
            {
                longPressGesture.StateChanged += longPressedHandler;
                //Debug.Log($"Subscribed to {child.name}'s LongPressGesture");
            }
        }
    }

    private void longPressedHandler(object sender, GestureStateChangeEventArgs e)
    {
        Debug.Log("Long press detected on child!");

        if (e.State == Gesture.GestureState.Recognized)
        {
            if (!popUpPanel.activeInHierarchy)
            {
                ShowUIPopup();
            }
            else
            {
                Debug.Log("Prefab is already built.");
            }
        }
    }

    private void ShowUIPopup()
    {
        popUpPanel.SetActive(true);

        Debug.Log("UI popup instantiated.");
    }
}
