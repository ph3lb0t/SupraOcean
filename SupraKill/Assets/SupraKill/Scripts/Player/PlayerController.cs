using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEditor;
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

    [Header("Sounds")]
    [SerializeField] AudioSource jumpSoundEffect;
    [SerializeField] AudioSource deathSoundEffect;
    [SerializeField] AudioSource throwSoundEffect;

    //BOMBA
    [SerializeField] GameObject thrownBomb;
    [SerializeField] float bombOffsetDist;
    [SerializeField] float throwBombOffsetHeight;
    [SerializeField] float bombThrowingForce;
    [SerializeField] float throwAngleLimit;

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
    [SerializeField] Vector2 mousePos;

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
        PlayerDirection();

        //TakeDamage();

        if (velocityY < 0.5)
        {
            playerRb.gravityScale = 4f;
        }
        else
        {
            playerRb.gravityScale = 2;
        }

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
        PlayerAttributes();
        Movement();

    }

    public void MovementAxis(InputAction.CallbackContext context)
    {
        moveAxis = context.ReadValue<Vector2>();
    }

    void PlayerAttributes()
    {
        //GROUNDCHECK
        isGrounded = Physics2D.OverlapArea(
                        new Vector2(groundCheck.transform.position.x - (groundedAreaLength / 2),
                                    groundCheck.transform.position.y - groundedAreaHeight),
                        new Vector2(groundCheck.transform.position.x + (groundedAreaLength / 2),
                                    groundCheck.transform.position.y + 0.01f),
                                    groundLayer);

        //MOUSE: Screen to world pos
        Vector2 screenPosition = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(screenPosition);

        velocityX = playerRb.velocity.x;
        velocityY = playerRb.velocity.y;
    }

    void Movement()
    {
        //Stopping
        if (isGrounded)
        {
            if (isAttacking)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x / 1.05f, playerRb.velocity.y);
            }
            else if (moveAxis.x == 0)
            {
                if (Math.Abs(playerRb.velocity.x) > stoppingSpeed)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x / 1.3f, playerRb.velocity.y);
                }
                else
                {
                    playerRb.velocity = new Vector2(0, velocityY);
                }
            }
        }

        //Accelerating
        if (moveAxis.x > 0 && playerRb.velocity.x < playerBaseSpeed * 5)
        {
            playerRb.AddForce(moveAxis * (playerBaseSpeed / 1.5f) * playerSpeed, ForceMode2D.Impulse);
        }
        else if (moveAxis.x < 0 && playerRb.velocity.x > -playerBaseSpeed * 5)
        {
            playerRb.AddForce(moveAxis * (playerBaseSpeed / 1.5f) * playerSpeed, ForceMode2D.Impulse);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started && !isAttacking)
        {
            anim.SetTrigger("attack");
            attackSoundEffect.Play();
            if (mousePos.x > groundCheck.transform.position.x)
            {
                AttackingRight = true;
            }
            else if (mousePos.x < groundCheck.transform.position.x)
            {
                AttackingRight = false;
            }
        }
    }

    void AttackAction()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(atkController.position, atkRadius);

        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Enemy"))
            {
                collider.transform.GetComponent<MeleeEnemy>().TakeDamage(atkDmg);
            }
        }
    }

    public void BombThrow(InputAction.CallbackContext context)
    {
        if (context.started && !isAttacking)
        {
            anim.SetTrigger("throwBomb");
            throwSoundEffect.Play();
            if (mousePos.x > groundCheck.transform.position.x)
            {
                AttackingRight = true;
            }
            else if (mousePos.x < groundCheck.transform.position.x)
            {
                AttackingRight = false;
            }
        }
    }

    public void BombThrowAction()
    {
        //Throwing origin position
        Vector2 throwPos = new Vector2(transform.position.x, transform.position.y + throwBombOffsetHeight);
        //Difference between origin and target
        Vector2 direction = mousePos - throwPos;
        //Value in radians for the direction of the throw, clamped between min and max to avoid traspassing ground
        float directionAngle = Mathf.Clamp(Mathf.Atan2(direction.x, direction.y), -Mathf.PI * throwAngleLimit, Mathf.PI * throwAngleLimit);
        //Normalized direction in Vector2 values, Y multiplied by -1 because it was inverted otherwise, don't know why
        Vector2 directionNormalized = new Vector2(Mathf.Cos(directionAngle - Mathf.PI / 2), - Mathf.Sin(directionAngle - Mathf.PI / 2));
        //Position from which to spawn the projectile
        Vector2 offset = directionNormalized * bombOffsetDist + throwPos;

        //Instantiate and give force to the projectile prefab in the right position with the right direction
        GameObject newBomb = Instantiate(thrownBomb, offset, transform.rotation);
        newBomb.GetComponent<Rigidbody2D>().AddForce(directionNormalized * bombThrowingForce, ForceMode2D.Force);
        //newBomb.GetComponent<Rigidbody2D>().()
    }

    private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(atkController.position, atkRadius);
        }


    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
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

        anim.SetTrigger("hurt");
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

    