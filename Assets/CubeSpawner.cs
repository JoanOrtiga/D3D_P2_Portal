using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cube;
    public Transform spawnPoint;
    // Start is called before the first frame update
    public void SpawnCube()
    {
        Instantiate(cube, spawnPoint.position, Quaternion.identity);
    }
}
