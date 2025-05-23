using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;

public class DrillTaskCaller : MonoBehaviour
{
    public GameObject popupPrefab; // Assign the UI popup prefab
    public GameManager gameManager;
    public GameObject IceModel;

    private string gpName;
    private GameObject currentPopup;

    private Vector3[] directions =
    {
        new Vector3(3, 5, -3),
        new Vector3(5, 5, -3),
        new Vector3(3, 5, -6),
    };
    
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
            ShowUIPopup();
        }
    }

    private void ShowUIPopup()
    {
        if (popupPrefab != null)
        {
            if (currentPopup == null)
            {
                currentPopup.SetActive(true);
                Debug.Log($"Supplies UI instantiated");
            }
            else
            {
                Debug.Log("Popup already exists.");
            }
        }
        else
        {
            Debug.LogError("Popup prefab or target canvas is missing!");
        }
    }

    public void ActivateDrill()
    {
        Debug.Log($"{gameObject.name}: Data received, spawning ice.");

        foreach (Vector3 spawnPosition in directions)
        {
            GameObject ice = Instantiate(IceModel, transform.position + spawnPosition, Quaternion.identity);
            ice.name = "IceChunk";
            ice.transform.parent = transform.parent;
            ice.SetActive(true);
        }
    }
}
