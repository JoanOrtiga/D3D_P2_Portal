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
        print(gamemanager.paused);
        if(gamemanager.paused == false)
        {
            print(gamemanager.paused);

            gameObject.SetActive(false);
        }
    }
}
