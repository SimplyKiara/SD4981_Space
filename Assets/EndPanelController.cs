using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndPanelController : MonoBehaviour
{
    public Button myButton;
    
    void Start()
    {
        myButton.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        Debug.Log("Item called");
    }
}
