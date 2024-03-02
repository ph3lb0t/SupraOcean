using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canonscript : MonoBehaviour
{
    public Transform firepoint;
    public GameObject bullet;
    [SerializeField] float timebetween;
    public float starttimebetween;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        timebetween = starttimebetween;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timebetween <= 0)
        {
            anim.SetTrigger("attack");
            timebetween = starttimebetween;
        }
        else
        {
            timebetween -= Time.deltaTime;
        }
    }

    void ATK()
    {
        
        GameObject newBullet = Instantiate(bullet, firepoint.position, firepoint.rotation);
        newBullet.transform.localScale = transform.localScale;
    }
}
