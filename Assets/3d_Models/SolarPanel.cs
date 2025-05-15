using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPanel : MonoBehaviour
{
    public bool panelSet = false;

    public void SetActiveState(bool state)
    {
        panelSet = state;
        gameObject.SetActive(state);
    }
}
