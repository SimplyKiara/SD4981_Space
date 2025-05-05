using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject OriginalPanel;
    public GameObject nextPanel;

    void Start()
    {
        OriginalPanel.SetActive(true);
        nextPanel.SetActive(false);
    }
    public void Next() {
        OriginalPanel.SetActive(false);
        nextPanel.SetActive(true);
    }
}
