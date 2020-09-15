using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon : MonoBehaviour
{
    public Transform firePointRight;//Setting up Right-firePoint
    public Transform firePointLeft;//Setting up Left-firePoint

    public GameObject fireBallPrefeb;//Using fireball from Prefeb
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))//fire key setting
        {
            Shoot();
        }
        
    }

    void Shoot()//Shoot Logic
    {
        //Define which firePoint should be used(left/right) based on the Player facing
        if(transform.localScale == new Vector3(1, 1, 1)) {
            Instantiate(fireBallPrefeb, firePointRight.position, firePointRight.rotation);
        }
        if (transform.localScale == new Vector3(-1, 1, 1))
        {
            Instantiate(fireBallPrefeb, firePointLeft.position, firePointLeft.rotation);
        }

        
    }
}
