using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public GameObject door;
    
   
    private void OnTriggerEnter(Collider other)
    {
       
            door.GetComponent<Animator>().SetBool("open", true);
        
    }
    private void OnTriggerExit(Collider other)
    {
        
            door.GetComponent<Animator>().SetBool("open", false);
        
    }
}
