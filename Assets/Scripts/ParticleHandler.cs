using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ParticleHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public Button ThrustButton;
    public ParticleSystem particle;
    void Start()
    {
        particle.Stop();
        ThrustButton.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnButtonClick() {
        particle.Play();
    }
}
