using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRb;
    private Animator anim;
    [SerializeField] float horizontalInput;
    [SerializeField] private AudioSource jumpSoundEffect;
    [SerializeField] private AudioSource deathSoundEffect;

    [Header("Player Attributes")]
    [SerializeField] float velocityY;
    [SerializeField] float velocityX;
    [SerializeField] float playerBaseSpeed;
    public float playerSpeed;
    [SerializeField] float stoppingSpeed;

    public float jumpForce;

    public float gravityX;
    public float gravityY;

    public float maxHealth = 100;
    public float currentHealth;


    [Header("Player Direction")]
    [SerializeField] bool isFacingRight = true;
    [SerializeField] Vector2 moveAxis;

    [Header("Ground Check")]
    [SerializeField] float groundedAreaLength;
    [SerializeField] float groundedAreaHeight;
    [SerializeField] bool isGrounded;
    [SerializeField] GameObject groundCheck;
    [SerializeField] LayerMask groundLayer;

    [Header("Attack")]
    [SerializeField] bool isAttacking;
    [SerializeField] bool AttackingRight;

    [SerializeField] float atkMoveSpeed;
    [SerializeField] private Transform atkController;
    [SerializeField] private float atkRadius;
    [SerializeField] private float atkDmg;
    [SerializeField] private float atkCD;
    [SerializeField] private float timeUntilAttack;

    [SerializeField] private AudioSource attackSoundEffect;

    [Header("Mouse Position")]
    [SerializeField] Vector3 mousePos;

    private void Awake()
    {
        groundCheck = GameObject.Find("GroundCheck");
        playerRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        Physics2D.gravity = new Vector2(gravityX, gravityY);
    }

    //TRAMPA

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            Death();
        }
    }

    void Update()
    {
        PlayerAttributes();
        
        //TakeDamage();

        if (velocityY < 0.5)
        {
            playerRb.gravityScale = 4f;
        }
        else
        {
            playerRb.gravityScale = 2;
        }
        //GROUNDCHECK
        isGrounded = Physics2D.OverlapArea(
                        new Vector2(groundCheck.transform.position.x - (groundedAreaLength / 2),
                                    groundCheck.transform.position.y - groundedAreaHeight),
                        new Vector2(groundCheck.transform.position.x + (groundedAreaLength / 2),
                                    groundCheck.transform.position.y + 0.01f),
                                    groundLayer);

        Vector3 screenPosition = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(screenPosition);
        mousePos.z = 0;

        if (isAttacking)
        {
            playerSpeed = atkMoveSpeed;
        }
        else
        {
            playerSpeed = 1;
        }

        ConstantSaiAnim();
        TriggerAnimations();



    }


    private void FixedUpdate()
    {
        Movement();
        PlayerDirection();

    }

    public void MovementAxis(InputAction.CallbackContext context)
    {
        moveAxis = context.ReadValue<Vector2>();
    }

    void Movement()
    {
        //Frenada
        if (isGrounded)
        {
            if (isAttacking)
            {
                playerRb.velocity = new Vector2(velocityX / 1.05f, velocityY);
            }
            else if (moveAxis.x == 0)
            {
                if (Math.Abs(velocityX) > stoppingSpeed)
                {
                    playerRb.velocity = new Vector2(velocityX / 1.3f, velocityY);
                }
                else
                {
                    playerRb.velocity = new Vector2(0, velocityY);
                }
            }
        }

        if (moveAxis.x > 0 && velocityX < playerBaseSpeed * 5)
        {
            playerRb.AddForce(moveAxis * (playerBaseSpeed / 1.5f) * playerSpeed, ForceMode2D.Impulse);
        }
        else if (moveAxis.x < 0 && velocityX > -playerBaseSpeed * 5)
        {
            playerRb.AddForce(moveAxis * (playerBaseSpeed / 1.5f) * playerSpeed, ForceMode2D.Impulse);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!isAttacking)
        {
            anim.SetTrigger("attack");
            if (mousePos.x > groundCheck.transform.position.x)
            {
                AttackingRight = true;
                Atk();
            }
            else if (mousePos.x < groundCheck.transform.position.x)
            {
                AttackingRight = false;
                Atk();
            }
        }

        void Atk() 
        { 
        attackSoundEffect.Play();
        Collider2D[] objects = Physics2D.OverlapCircleAll(atkController.position, atkRadius);

            foreach (Collider2D collider in objects)
            {
                if (collider.CompareTag("Enemy"))
                {
                collider.transform.GetComponent<MeleeEnemy>().TakeDamage(atkDmg);
                }
            }
        }
    }
    void Atk()
    {
        attackSoundEffect.Play();
        Collider2D[] objects = Physics2D.OverlapCircleAll(atkController.position, atkRadius);

        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Enemy"))
            {
                collider.transform.GetComponent<MeleeEnemy>().TakeDamage(atkDmg);
            }
        }
    }



private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(atkController.position, atkRadius);

    }

    void PlayerAttributes()
    {
        velocityX = playerRb.velocity.x;
        velocityY = playerRb.velocity.y;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            jumpSoundEffect.Play();
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
    }


    void PlayerDirection()
    {
        horizontalInput = moveAxis.x;
        //Left Direction
        if (isAttacking && !AttackingRight && isFacingRight) { Flip(); }
        else if (!isAttacking && horizontalInput < 0 && isFacingRight) { Flip(); }
        //Right Direction
        if (isAttacking && AttackingRight && !isFacingRight) { Flip(); }
        else if (!isAttacking && horizontalInput > 0 && !isFacingRight) { Flip(); }
    }

    void Flip()
    {
        Vector2 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }

    void ConstantSaiAnim()
    {
        //run ANIMATION
        if (Math.Abs(playerRb.velocity.x) > 1)
        { anim.SetBool("run", true); }
        else { anim.SetBool("run", false); }

        //MIDAIR ANIMATION
        if (!isGrounded)
        { anim.SetBool("jump", true); }
        else { anim.SetBool("jump", false); }
    }

    public void SaiTakeDamage(int amount)
    {
        Debug.Log("player hit");

        currentHealth -= amount;

        if (currentHealth <= 0) 
        {
            Death();
        }
    }

    private void Death()
    {
        deathSoundEffect.Play();
        playerRb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
        //Then show death screen
    }

    //RESTART

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void TriggerAnimations()
    {

    }

    void IsAttackingEvent()
    { isAttacking = true; }

    void IsNotAttackingEvent()
    { isAttacking = false; }

}

    