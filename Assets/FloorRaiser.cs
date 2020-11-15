using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorRaiser : MonoBehaviour
{
    public GameObject floor;
    private bool onButton = true;
    private void Update()
    {

      
    }

    private void OnTriggerEnter(Collider other)
    {
       
            floor.GetComponent<Animator>().SetBool("Raise", true);
        
    }
    private void OnTriggerExit(Collider other)
    {
        
            floor.GetComponent<Animator>().SetBool("Raise", false);
        
    }
}
