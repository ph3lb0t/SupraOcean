using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    [Header("General Data")]
    public int score;
    public int maxScore;
    public int health;
    public int energy;

    [Header("Game Status")]
    public bool gameCompleted = false;

}
