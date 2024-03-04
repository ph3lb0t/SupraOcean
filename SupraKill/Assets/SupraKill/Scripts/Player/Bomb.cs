using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionForce;

    public float Speed = 4;
    public Vector3 LaunchOffset;
    public bool Thrown;

    Rigidbody2D rb;
    CircleCollider2D col;

    private void Start()
    {
        Destroy(gameObject, 5); // Destroy automatically after 5 seconds
    }

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Explosion()
    {
        gameObject.transform.eulerAngles = new Vector3 (transform.rotation.x, transform.rotation.y, 0);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;

        col.isTrigger = true;

        Collider2D[] tempRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D collider in tempRadius)
        {

            Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = collider.transform.position - transform.position;
                float distance = 2 + direction.magnitude;
                float finalForce = 400 * explosionForce / distance;
                rb.AddForce(direction * finalForce);
            }
        }
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
