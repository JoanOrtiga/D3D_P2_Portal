using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CubeButton : MonoBehaviour
{
    public UnityEvent buttonPressed;
    void OnTriggerEnter(Collider _Collider)
    {
        if (_Collider.tag == "Cube")
            buttonPressed.Invoke();
    }
}
