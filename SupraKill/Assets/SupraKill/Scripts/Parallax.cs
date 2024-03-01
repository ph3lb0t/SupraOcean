using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] float length, startPos;
    public GameObject cam;
    SpriteRenderer spriteRenderer;
    public float parallaxEffect;
    [SerializeField] float temp, dist;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPos = transform.position.x;
        length = spriteRenderer.bounds.size.x;
    }

    private void FixedUpdate()
    {
        temp = (cam.transform.position.x * (1 - parallaxEffect));
        dist = (cam.transform.position.x * parallaxEffect);
        
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        if (temp > startPos + length)
        {
            startPos += 2 * length;
        }
        else if (temp < startPos - length)
        {
            startPos -= 2 * length;
        }
    }
}
