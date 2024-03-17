using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    Animator anim;
    PlayerSFX sfx;

    public float maxHealth;
    public float currentHealth;
    bool invulnerable;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sfx = GetComponent<PlayerSFX>();
    }
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void HealthChange(int amount)
    {
        if (!invulnerable)
        {
            Invoke("InvulnOff", .5f);
            anim.SetTrigger("hurt");
            currentHealth += amount;

            if (currentHealth <= 0)
            {
                Death();
            }
        }

        void InvulnOff()
        {
            invulnerable = false;
        }

    }

    private void Death()
    {
        invulnerable = true;
        sfx.SFX[0].Play();
        anim.SetTrigger("death");
        //Then show death screen
    }

    void Update()
    {
        
    }
}
