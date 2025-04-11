using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyBuilder : MonoBehaviour
{
    public void BuildStructure(int item)
    {
        switch (item)
        {
            case 0:
                GameManager.instance.BuildSolarPanels();
                break;
            case 1:
                GameManager.instance.UpgradeBase();
                break;
            case 2:
                GameManager.instance.BuildGreenhouse();
                break;
            default:
                Debug.Log("Action Unknown");
                break;
        }
    }
}
