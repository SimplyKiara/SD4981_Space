using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseToSiteController : MonoBehaviour
{
    public WallConnection wallConnection;
    public Text descriptionText;

    public void BaseToSite()
    {
        wallConnection.ChangeScene("ResourcesScene", "Teacher");
        descriptionText.text = "Request submitted.\nClosing panel:";
        Invoke("UIDeactivator", 2.5f);
    }

    public void UIDeactivator()
    {
        gameObject.SetActive(false);
    }
}
