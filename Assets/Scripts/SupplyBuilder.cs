using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyBuilder : MonoBehaviour
{
    public GameManager manager;

    public void BuildStructure(int item)
    {
        switch (item)
        {
            case 0:
                manager.BuildSolarPanels();
                break;
            case 1:
                manager.UpgradeBase();
                break;
            case 2:
                manager.BuildGreenhouse();
                break;
            default:
                Debug.Log("Action Unknown");
                break;
        }
    }
}
