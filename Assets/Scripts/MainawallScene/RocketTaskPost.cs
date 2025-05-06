using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTaskPost : MonoBehaviour
{
    public WallConnection wallConnection;
    void OnDestroy()
    {
        wallConnection.TriggerTask("Rocket Landing", "Group 1");
    }
}
