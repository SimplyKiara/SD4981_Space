using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanderActive : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject spaceship;
    public void Active() {
        spaceship.SetActive(true);
    }
}
