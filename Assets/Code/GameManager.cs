using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{
    public GameObject gameOver;
    public List<RestartableObject> restartableObjects;

    public bool paused;

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
        paused = true;
        Cursor.lockState = CursorLockMode.Confined;
        gameOver.SetActive(true);
      //  Time.timeScale = 0;
    }

    public void LoadNext()
    {
        SceneManager.LoadScene("SecondScene");

    }
   
    public void RestartLevel()
    {

       // Time.timeScale = 1;
        paused = false;

        Cursor.lockState = CursorLockMode.Locked;
        
        for (int i = 0; i < restartableObjects.Count; i++)
        {
            restartableObjects[i].RestartObject();
        }

 
    }

}
