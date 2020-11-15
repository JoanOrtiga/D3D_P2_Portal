using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    GameManager gamemanager;
    // Update is called once per frame

    private void Start()
    {
        gamemanager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (gamemanager.paused == false)
        {
            gameObject.SetActive(false);
        }
    }
}
