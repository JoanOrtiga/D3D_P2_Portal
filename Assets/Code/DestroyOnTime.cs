using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    public float destroyTime = 3.0f;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
