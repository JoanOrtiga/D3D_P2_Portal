using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCubes : MonoBehaviour
{
    private bool pulsado;
    public CubeSpawner spawner;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pulsado)
                spawner.SpawnCube();

        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            pulsado = true;
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            pulsado = false;
        }
    }
}
