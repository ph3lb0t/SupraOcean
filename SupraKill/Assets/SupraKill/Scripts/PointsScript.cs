using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsScript : MonoBehaviour
{
    public int pointSum;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.PointsUp(pointSum);
            gameObject.SetActive(false);
        }
    }
}
