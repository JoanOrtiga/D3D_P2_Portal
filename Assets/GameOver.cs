using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    GameManager gameManager;
    // Update is called once per frame

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (gameManager.paused == false)
        {
            gameObject.SetActive(false);
        }
    }
}
