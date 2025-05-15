using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseToSiteController : MonoBehaviour
{
    public ClientConnection clientConnection;
    public Text descriptionText;

    private void OnEnable()
    {
        clientConnection = GameObject.Find("Client Manager").GetComponent<ClientConnection>();
    }

    public void BaseToSite()
    {
        //wallConnection.ChangeScene("ResourcesScene", "Teacher");
        descriptionText.text = "Request submitted.\nClosing panel:";
        Invoke("UIDeactivator", 2.5f);
    }

    public void UIDeactivator()
    {
        gameObject.SetActive(false);
    }
}
