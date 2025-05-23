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

    void PotAction()
    {
        Collider2D collider = Physics2D.OverlapPoint(transform.position);

        if (collider != null && collider.gameObject != gameObject)
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
            }
            else if (collider.gameObject.CompareTag("Seed"))
            {
                if (!haveSeed && haveMud)
                {
                    haveSeed = true;
                    animator.SetBool("Seeded", true);
                    Debug.Log("Seed planted!");
                }
                else
                {
                    Debug.Log("Can't plant seed!");
                }
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
            }
            else
            {
                Debug.Log("Unknown action type");
            }
        }
    }
}
