using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    private int bombita = 0;

    [SerializeField] private Text bombitaText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bombita"))
        {

            Destroy(collision.gameObject);
            bombita++;
            bombitaText.text = "Bombas: " + bombita; 
        }
    }
}
