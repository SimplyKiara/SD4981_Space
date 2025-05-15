using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvManager : MonoBehaviour
{
    public float RotateSpeed = -0.8f;
    public GameObject earth;
    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotateSpeed);
        earth.transform.Rotate(Vector3.up * 1 * Time.deltaTime);
    }
}
