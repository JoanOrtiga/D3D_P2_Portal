using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public GameObject door;
    private AudioSource sonido;
    private void Start()
    {
        sonido = door.GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        sonido.Play();

    }
    private void OnTriggerStay(Collider other)
    {
       
            door.GetComponent<Animator>().SetBool("open", true);
        
    }
    private void OnTriggerExit(Collider other)
    {
        
            door.GetComponent<Animator>().SetBool("open", false);
        sonido.Play();
    }
}
