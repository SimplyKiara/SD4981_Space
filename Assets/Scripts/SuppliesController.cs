using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.UI;

public class SuppliesController : MonoBehaviour
{
    public GameObject popupPrefab; // Assign the UI popup prefab

    private string gpName;

    private void Start()
    {
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
            ShowUIPopup();
        }
    }

    private void ShowUIPopup()
    {
        if (popupPrefab != null)
        {
            popupPrefab.SetActive(true);
            Debug.Log($"Supplies UI shown");
        }
        else
        {
            Debug.LogError("Popup is missing!");
        }
    }

    public void DeactivatePopUp()
    {
        gameObject.SetActive(false);
    }
}
