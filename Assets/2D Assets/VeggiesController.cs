using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VeggiesController : MonoBehaviour
{
    public bool collected = false;
    public bool watered = false;

    public bool HvCollected => collected;
    public bool HvWatered => watered;

    private Animator animator;
    private static int collectedVeggies = 0;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (!collected)
        {
            animator.SetBool("Cut", false);
            animator.SetBool("Watered", false);
        }
        else
        {
            if (!watered)
            {
                animator.SetBool("Cut", true);
                animator.SetBool("Watered", false);
            }
            else
            {
                animator.SetBool("Cut", true);
                animator.SetBool("Watered", true);
            }
        }
    }

    void VeggiesAction()
    {
        Collider2D collider = Physics2D.OverlapPoint(transform.position);

        if (collider != null && collider.gameObject != gameObject)
        {
            if (collider.gameObject.CompareTag("Cutters")) 
            {
                if (!collected)
                {
                    collected = true;
                    animator.SetBool("Cut", true);
                    animator.SetBool("Watered", false);
                    //Debug.Log("Veggie collected!");
                    collectedVeggies++;
                }
                else
                {
                    Debug.Log("Already collected!");
                }
            }
            else if (collider.gameObject.CompareTag("WaterPipette"))
            {
                if (!watered)
                {
                    if (collected)
                    {
                        watered = true;
                        animator.SetBool("Cut", true);
                        animator.SetBool("Watered", true);
                        collider.gameObject.SendMessage("PipeHandler");
                        //Debug.Log("Watered veggie!");
                    }
                    else
                    {
                        Debug.Log("Not collected yet!");
                    }
                }
                else
                {
                    Debug.Log("Already watered veggie!");
                }
            }
        }
    }
}
