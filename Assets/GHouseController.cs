using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TouchScript.Gestures;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;


public class GHouseController : MonoBehaviour
{
    public GameObject popUpPanel; // Assign the UI popup
    public WallConnection wallConnection;

    private string gpName;

    private void Start()
    {
        popUpPanel.SetActive(false);
        gpName = gameObject.name.Substring(12,1);  // Initialise group name

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

    // Unlock harvesting and deleting
    public void CallHarvest()
    {
        wallConnection.TriggerTask("Harvesting", "Group " + gpName);
        wallConnection.DeleteTask("Planting", "Group " + gpName);
    }
}
