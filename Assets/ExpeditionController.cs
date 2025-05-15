using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionController : MonoBehaviour
{
    public GameObject closingPanel;
    public WallConnection wallConnection;

    private bool triggered = false;

    // Update is called once per frame
    void Update()
    {
        if (!triggered)
        {
            if (closingPanel == null)
            {
                triggered = true;
                wallConnection.TriggerTask("Expedition", "Group 1");
                wallConnection.TriggerTask("Expedition", "Group 2");
                wallConnection.TriggerTask("Expedition", "Group 3");
            }        
        }
    }
}
