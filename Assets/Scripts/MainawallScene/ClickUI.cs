using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickUI : MonoBehaviour
{
    public Transform refObj;
    bool isActive;
    void Start()
    {
        if (refObj.name == "coveringCrater")
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
        if (isActive == false)
        {
            Destroy(gameObject);
        }


    }
}
