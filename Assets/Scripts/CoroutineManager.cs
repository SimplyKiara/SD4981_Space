using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    public static CoroutineManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDisableAndEnable(GameObject target)
    {
        StartCoroutine(DisableAndEnable(target));
    }

    IEnumerator DisableAndEnable(GameObject target)
    {
        target.SetActive(false);
        //Debug.Log("GameObject set to false");
        yield return new WaitForSeconds(0.5f);
        //Debug.Log("5 seconds have passed");
        target.SetActive(true);
        //Debug.Log("GameObject set to true from false");
    }
}
