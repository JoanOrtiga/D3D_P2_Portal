using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CubeButton : MonoBehaviour
{
    public UnityEvent buttonPressed;
    public UnityEvent buttonUnpressed;

    private bool canBeUnpressed;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
            buttonPressed.Invoke();
        else if (other.CompareTag("Player"))
            buttonPressed.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cube") || other.CompareTag("Player"))
        {
            canBeUnpressed = false;
        }
        else
            canBeUnpressed = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (canBeUnpressed)
        {
            if (other.CompareTag("Cube"))
            {
                buttonUnpressed.Invoke();
            }
            else if (other.CompareTag("Player"))
            {
                buttonUnpressed.Invoke();
            }
        }
    }
}
