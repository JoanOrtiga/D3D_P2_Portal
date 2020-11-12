using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionSpawner : MonoBehaviour
{
    public GameObject companionPrefab;
    public Transform spawnerPosition;

    private GameObject currentCube;

    bool spawnerActive = false;
    private void Spawn()
    {
        currentCube = GameObject.Instantiate(companionPrefab, spawnerPosition.position, spawnerPosition.rotation);
    }

    private void Update()
    {
        if(currentCube == null && spawnerActive)
        {
            Spawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spawnerActive = true;
            Spawn();
        }
    }
}
