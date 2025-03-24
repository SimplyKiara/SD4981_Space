using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Meteor : MonoBehaviour
{
    public float destroyDelay = 2f;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}
