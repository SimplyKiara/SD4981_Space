using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotController : MonoBehaviour
{
    private Animator animator;

    private bool haveMud = false;
    private bool haveSeed = false;
    private bool watered = false;

    public bool HaveMud => haveMud;    // Expose haveMud
    public bool HaveSeed => haveSeed; // Expose haveSeed
    public bool Watered => watered;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    void PotAction()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag("Mud"))
            {
                if (!haveMud)
                {
                    haveMud = true;
                    animator.SetBool("Filled", true);
                    Debug.Log("Current value of Filled:" + animator.GetBool("Filled"));
                }
                else
                {
                    Debug.Log("Already have mud!");
                }
                break;
            }
            else if (collider.gameObject.CompareTag("Seed"))
            {
                if (!haveSeed && haveMud)
                {
                    haveSeed = true;
                    Debug.Log("Seed planted!");
                }
                else
                {
                    Debug.Log("Can't plant seed!");
                }
                break;
            }
            else if (collider.gameObject.CompareTag("WaterPipette"))
            {
                if (!watered && haveMud && haveSeed)
                {
                    watered = true;
                    animator.SetBool("Watered", true);
                    Debug.Log("Current value of Watered:" + animator.GetBool("Watered"));
                    collider.gameObject.SendMessage("PipeHandler");
                }
                else
                {
                    Debug.Log("Cannot water!");
                }
                break;
            }
            else
            {
                Debug.Log("Unknown action type");
            }
        }
    }
}
