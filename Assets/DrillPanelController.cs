using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.UI;

public class DrillPanelController : MonoBehaviour
{
    public WallConnection wallConnection;

    public GameObject title;
    public GameObject announcement;
    public GameObject confirm;
    public GameObject close;

    private void Start()
    {
        title.SetActive(true);
        announcement.SetActive(false);
        confirm.SetActive(true);
        close.SetActive(false);
    }

    public void CallTask()
    {
        if (wallConnection != null)
        {
            wallConnection.TriggerTask("Ice Mining", "Group 1");

            title.SetActive(false);
            announcement.SetActive(true);
            confirm.SetActive(false);
            close.SetActive(true);
        }
    }

    public void DisablePopUp()
    {
        gameObject.SetActive(false);
    }
}
