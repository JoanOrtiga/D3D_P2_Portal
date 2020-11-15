using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //public GameOver gameOver;

    public List<RestartableObject> restartableObjects;

    private void Start()
    {
        restartableObjects = new List<RestartableObject>();

        foreach (RestartableObject item in FindObjectsOfType<RestartableObject>())
        {
            restartableObjects.Add(item);
        }
    }

    public void GameOver()
    {

    }

    public void RestartLevel()
    {
        for (int i = 0; i < restartableObjects.Count; i++)
        {
            restartableObjects[i].RestartObject();
        }
    }

}
