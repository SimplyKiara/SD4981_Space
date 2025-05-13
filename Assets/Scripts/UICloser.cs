using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICloser : MonoBehaviour
{
    public GameObject immovable;

    public void ClosePopup()
    {
        Debug.Log("UI popup closed.");

        if (immovable != null)
        {
            immovable.SetActive(true);
            Debug.Log("Object can move now");
        }

        Destroy(gameObject);
    }
}
