using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float range;
    [SerializeField] private int atkDmg;
    [SerializeField] private bool playerInRange;
    [SerializeField] bool isAttacking;



    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth;
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;

    [Header("Components")]
    [SerializeField] private Behaviour[] components;
    private bool invulnerable;

    //References
    private Animator anim;
    private BoxCollider2D boxCollider;
    private MeleeEnemy playerHealth;
    private EnemyPatrol enemyPatrol;

    //External References
    private GameObject sai;

    private void Awake()
    {
        sai = GameObject.Find("Sai");
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Physics2D.OverlapArea(new Vector2(transform.position.x - .5f, boxCollider.bounds.min.y), new Vector2(transform.position.x + range, boxCollider.bounds.max.y), LayerMask.GetMask("Player")))
        { playerInRange = true; }
        else { playerInRange = false; }

        if (playerInRange && !isAttacking)
        {
            anim.SetTrigger("attack");
        }
    }

    void Attack()
    {
        Debug.Log("attack done");
        if (playerInRange)
        {
            sai.GetComponent<PlayerController>().SaiTakeDamage(atkDmg);
        }
    }

    public void TakeDamage(float _damage)
    {
        if (!invulnerable) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            StartCoroutine(Invunerability());
        }
        else
        {
            if (!dead)
            {
                anim.SetTrigger("death");

                //Deactivate all attached component classes
                foreach (Behaviour component in components)
                    component.enabled = false;

                dead = true;
            }
        }
    }

    private IEnumerator Invunerability()
    {
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }
   
    void isAttackingTrue()
    {
        isAttacking = true;
    }
    void isAttackingFalse()
    {
        isAttacking = false;
    }
}
