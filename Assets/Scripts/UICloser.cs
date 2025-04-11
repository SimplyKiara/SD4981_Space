using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICloser : MonoBehaviour
{
    public void ClosePopup()
    {
        Debug.Log("UI popup closed.");
        Destroy(gameObject);
    }
}
