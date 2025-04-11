using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.UI;

public class SuppliesController : MonoBehaviour
{
    public GameObject popupPrefab; // Assign the UI popup prefab
    public Canvas targetCanvas; // Assign the specific UI Canvas
    private GameObject currentPopup;
    private bool isBuilt = false;

    private void Start()
    {
        // Find all child objects with LongPressGesture
        foreach (Transform child in transform)
        {
            LongPressGesture longPressGesture = child.GetComponent<LongPressGesture>();

            if (longPressGesture != null)
            {
                longPressGesture.StateChanged += longPressedHandler;
                Debug.Log($"Subscribed to {child.name}'s LongPressGesture");
            }
        }
    }

    private void longPressedHandler(object sender, GestureStateChangeEventArgs e)
    {
        Debug.Log("Long press detected on child!");

        if (e.State == Gesture.GestureState.Recognized)
        {
            if (!isBuilt)
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
        if (popupPrefab != null && targetCanvas != null)
        {
            if (currentPopup == null)
            {
                currentPopup = Instantiate(popupPrefab, targetCanvas.transform, false);
                Debug.Log("UI popup instantiated.");
            }
            else
            {
                Debug.Log("Popup already exists, not creating a new one.");
            }
        }
        else
        {
            Debug.LogError("Popup prefab or target canvas is missing!");
        }
    }
}
