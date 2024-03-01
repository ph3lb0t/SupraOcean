using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("General References")]
    //[SerializeField] Image healthBar;

    [Header("Health Parameters")]
    public float health;
    public float maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0) { health = 0; }
        //healthBar.fillAmount = health / maxHealth;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }
}
