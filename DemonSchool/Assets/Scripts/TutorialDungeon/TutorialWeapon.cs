using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialWeapon : MonoBehaviour
{
    public Transform firePointRight;//Setting up Right-firePoint
    public Transform firePointLeft;//Setting up Left-firePoint
    public Transform firePointUp;
    public Transform firePointDown;

    public GameObject fireBallPrefeb;//Using fireball from Prefeb


    // Update is called once per frame
    void Start()
    {
        GetComponent<TutorialPlayerBehaviour>().playerAmmoNum.text = GetComponent<TutorialPlayerBehaviour>().playerAmmo.ToString();
        GetComponent<TutorialPlayerBehaviour>().playerHealthNum.text = GetComponent<TutorialPlayerBehaviour>().playerHealth.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))//fire key setting
        {
            if(GetComponent<TutorialPlayerBehaviour>().playerAmmo != 0)//Player will not allowed to shoot unless the ammo is > 0
            {
                Shoot();
                GetComponent<TutorialPlayerBehaviour>().playerAmmo -= 1;//EveryTime Press shoot key it will makes ammo -1
                GetComponent<TutorialPlayerBehaviour>().playerAmmoNum.text = GetComponent<TutorialPlayerBehaviour>().playerAmmo.ToString();
            }
        }
        
    }

    void Shoot()//Shoot Logic
    {

        if (GetComponent<TutorialPlayerBehaviour>().jockDirection == 6)
        {
            Instantiate(fireBallPrefeb, firePointRight.position, firePointRight.rotation);
        }
        else if (GetComponent<TutorialPlayerBehaviour>().jockDirection == 9)
        {
            Instantiate(fireBallPrefeb, firePointLeft.position, firePointLeft.rotation);
        }
        else if(GetComponent<TutorialPlayerBehaviour>().jockDirection == 3)
        {
            Instantiate(fireBallPrefeb, firePointUp.position, firePointUp.rotation);
        }
        else if (GetComponent<TutorialPlayerBehaviour>().jockDirection == 0)
        {
            Instantiate(fireBallPrefeb, firePointDown.position, firePointDown.rotation);
        }


    }
}
