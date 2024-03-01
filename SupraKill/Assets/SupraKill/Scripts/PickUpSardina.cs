using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSardina : MonoBehaviour
{
    PlayerController playerHealth;
    public float healthBonus = 15f;
    [SerializeField] private AudioSource healSoundEffect;

    private void Awake()
    {
        playerHealth = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerHealth.currentHealth < playerHealth.maxHealth)
        {
            healSoundEffect.Play();
            Destroy(gameObject);
            playerHealth.currentHealth = playerHealth.currentHealth + healthBonus;
        }
    }
}
