using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Inicio del singleton básico
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log("AudioManager is null!");
            }
            return instance;
        }
    }

    //Fin del Singleton básico, se referencia en el awake

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (points >= winPoints)
        {
            LoadScene(sceneToLoad);
        }
    }

    //Variables

    [Header("General Data")]
    public int points;
    public int winPoints;
    public int sceneToLoad;
    public int score;
    public int maxScore;
    public int health;
    public int energy;

    public void PointsUp(int gain)
    {
        points += gain;
    }

    public void LoadScene(int sceneIndex)
    {
       SceneManager.LoadScene(sceneIndex);
    }

    [Header("Game Status")]
    public bool gameCompleted = false;

}
