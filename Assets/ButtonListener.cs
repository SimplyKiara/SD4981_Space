using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonListener : MonoBehaviour
{
    public Button myButton; // Assign this in the Inspector or find it dynamically

    void Start()
    {
        // Add a listener to the button
        myButton.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        Debug.Log("Button clicked!");
        // Add your desired functionality here
    }
}
