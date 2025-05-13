using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTaskPost : MonoBehaviour
{
    public string group;
    public WallConnection wallConnection;
    public void PostTask()
    {
        wallConnection.TriggerTask("Rocket Landing", group);
    }
}
