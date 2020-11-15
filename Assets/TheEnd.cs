using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheEnd : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject canvas;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        canvas.SetActive(true);
        Time.timeScale=0;
    }
}
