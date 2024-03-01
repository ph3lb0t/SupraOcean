using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float Speed = 4;
    public Vector3 LaunchOffset;
    public bool Thrown;

    private void Start()
    {
        if (Thrown)
        {
            var direction = -transform.right + Vector3.up;
            GetComponent<Rigidbody2D>().AddForce(direction * Speed, ForceMode2D.Impulse);
        }
        transform.Translate(LaunchOffset);

        Destroy(gameObject, 5); // Destroy automatically after 5 seconds
    }

    public void Update()
    {
        if (!Thrown)
        {
            transform.position += -transform.right * Speed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var enemy = collision.collider.GetComponent<Health>();
        if (enemy)
        {
            enemy.TakeDamage(1);
        }
        Destroy(gameObject);
    }
}
