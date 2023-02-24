using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePoint_Right;
    public Transform firePoint_Left;
    public GameObject bulletPrefab;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }
    void Shoot()
    {
        //Debug.Log("Fire");
        bool flip = sr.flipX;
        //Debug.Log(flip);
        if (flip)
        {
            Instantiate(bulletPrefab, firePoint_Left.position, firePoint_Left.rotation);
        }
        
        else Instantiate(bulletPrefab, firePoint_Right.position, firePoint_Right.rotation);
    }
}
