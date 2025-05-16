using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SiteToBaseController : MonoBehaviour
{
    public WallConnection wallConnection;
    public Text descriptionText;

    // Submitting request for changing from resource site to main base
    public void SiteToBase()
    {
        wallConnection.ChangeScene("MainWallScene", "Teacher");
        descriptionText.text = "Request submitted.\nClosing panel:";
        Invoke("UIDeactivator", 2.5f);
    }

    public void UIDeactivator()
    {
        gameObject.SetActive(false);
    }
}
