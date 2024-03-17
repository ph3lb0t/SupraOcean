using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletscript : MonoBehaviour
{
    public float speed;
    bool playerDamaged;
    Rigidbody2D rb;
    [SerializeField] int atKDmg;


    // Start is called before the first frame update
    void Start()
    {
        rb.AddForce(new Vector2(speed * 10 * Mathf.Sign(transform.localScale.x), rb.velocity.y));
        Destroy(gameObject, 4f);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !playerDamaged)
        {
            rb.gravityScale = 1;
            playerDamaged = true;
            collision.gameObject.GetComponent<Health>().HealthChange(-atKDmg);
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
