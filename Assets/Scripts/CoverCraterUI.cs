using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverCraterUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject StartingUI;
    public GameObject CraterUI;
    void Start()
    {
        CraterUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (StartingUI == null) {
            Debug.Log("Starting UI has been destroyed!");
            CraterUI.SetActive(true);
        }
    }
}
