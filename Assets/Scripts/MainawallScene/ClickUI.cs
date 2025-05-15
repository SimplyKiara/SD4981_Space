using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickUI : MonoBehaviour
{
    public Transform refObj;
    bool isActive;
    bool haveUItoOpen = false;
    public GameObject refObj2;
    public GameObject refObj3;
    void Start()
    {
        if (refObj.name == "coveringCrater")
        {
            isActive = true;
            haveUItoOpen = true;
        }
        else if (refObj.name == "Supplies")
        {
            isActive = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (refObj == null)
        {
            isActive = false;
        }
        else if (!refObj.gameObject.activeInHierarchy)
        {
            isActive = false;
        }
        else if (refObj.name == "Supplies" && refObj2 == null)
        {
            refObj2 = refObj.parent.gameObject;
        }
        else if (refObj.name == "Supplies" && refObj2 != null && refObj3 == null)
        {
            if (refObj2.name.EndsWith("1"))
            {
                refObj3 = GameObject.FindWithTag("gp1_SuppliesPanel");
            }
            else if (refObj2.name.EndsWith("2"))
            {
                refObj3 = GameObject.FindWithTag("gp2_SuppliesPanel");
            }
            else if (refObj2.name.EndsWith("3"))
            {
                refObj3 = GameObject.FindWithTag("gp3_SuppliesPanel");
            }
        }
        if (refObj3 != null && refObj3.activeInHierarchy)
        {
            Destroy(gameObject);
        }
        if (isActive == false)
        {
            Destroy(gameObject);
        }


    }
}
