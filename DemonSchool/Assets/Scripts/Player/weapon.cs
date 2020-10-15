using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class weapon : MonoBehaviour
{
    public Transform firePointRight;//Setting up Right-firePoint
    public Transform firePointLeft;//Setting up Left-firePoint
    public Transform firePointUp;
    public Transform firePointDown;

    public GameObject fireBallPrefeb;//Using fireball from Prefeb

    
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))//fire key setting
        {
            if(GetComponent<PlayerBehaviour>().playerAmmo != 0)//Player will not allowed to shoot unless the ammo is > 0
            {
                Shoot();
                GetComponent<PlayerBehaviour>().playerAmmo -= 1;//EveryTime Press shoot key it will makes ammo -1
                GetComponent<PlayerBehaviour>().playerAmmoNum.text = GetComponent<PlayerBehaviour>().playerAmmo.ToString();


            }
        }
        
    }

    void Shoot()//Shoot Logic
    {
        //Define which firePoint should be used(left/right) based on the Player facing
        /*if(transform.localScale == new Vector3(1, 1, 1)) {
     
            Instantiate(fireBallPrefeb, firePointRight.position, firePointRight.rotation);
        }
        else if (transform.localScale == new Vector3(-1, 1, 1))
        {
            Instantiate(fireBallPrefeb, firePointLeft.position, firePointLeft.rotation);
        }*/

        if (GetComponent<PlayerBehaviour>().faceRight == true)
        {

            Instantiate(fireBallPrefeb, firePointRight.position, firePointRight.rotation);
        }
        else if (GetComponent<PlayerBehaviour>().faceLeft == true)
        {
            Instantiate(fireBallPrefeb, firePointLeft.position, firePointLeft.rotation);
        }
        else if(GetComponent<PlayerBehaviour>().faceUp == true)
        {
            Instantiate(fireBallPrefeb, firePointUp.position, firePointUp.rotation);
        }
        else if (GetComponent<PlayerBehaviour>().faceDown == true)
        {
            Instantiate(fireBallPrefeb, firePointDown.position, firePointDown.rotation);
        }


    }
}
