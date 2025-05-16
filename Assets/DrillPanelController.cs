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

    private string gpName;

    private void Start()
    {
        title.SetActive(true);
        announcement.SetActive(false);
        confirm.SetActive(true);
        close.SetActive(false);

        // Initialise group name
        if (gameObject.name.Substring(0,3) == "Gp1")
        {
            gpName = "Group 1";
        }
        else if (gameObject.name.Substring(0, 3) == "Gp2")
        {
            gpName = "Group 2";
        }
        else if (gameObject.name.Substring(0, 3) == "Gp3")
        {
            gpName = "Group 3";
        }
    }

    private void OnEnable()
    {
        wallConnection = GameObject.Find("ConnectionManager").GetComponent<WallConnection>();
    }

    // Unlock Ice Mining minigame
    public void CallTask()
    {
        if (wallConnection != null)
        {
            wallConnection.TriggerTask("Ice Mining", gpName);

            title.SetActive(false);
            announcement.SetActive(true);
            confirm.SetActive(false);
            close.SetActive(true);
        }
        else
        {
            Debug.LogError("Wall connection not found.");
        }
    }

    public void DisablePopUp()
    {
        gameObject.SetActive(false);
    }
}
