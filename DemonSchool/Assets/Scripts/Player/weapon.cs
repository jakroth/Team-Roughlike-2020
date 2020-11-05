using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public Transform firePointRight;//Setting up Right-firePoint
    public Transform firePointLeft;//Setting up Left-firePoint
    public Transform firePointUp;
    public Transform firePointDown;

    public GameObject fireBallPrefeb;//Using fireball from Prefeb

    private bool isPaused = false;

    void Update()
    {
        isPaused = GameController.instance.GetPauseState();

        if (Input.GetKeyDown(KeyCode.Space) && !isPaused) //fire key setting
        {
            GetComponent<PlayerBehaviour>().attackAnimation();
            if (GetComponent<PlayerBehaviour>().playerAmmo != 0) //Player will not allowed to shoot unless the ammo is > 0
            {
                Shoot();
                GetComponent<PlayerBehaviour>().playerAmmo -= 1; //EveryTime Press shoot key it will makes ammo -1
                GetComponent<PlayerBehaviour>().playerAmmoNum.text = GetComponent<PlayerBehaviour>().playerAmmo.ToString();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isPaused) //fire key setting
        {
            GetComponent<PlayerBehaviour>().daggerAttackAnimation();

        }
    }

    void Shoot()//Shoot Logic
    {

        if (GetComponent<PlayerBehaviour>().jockDirection == 10)
        {
            Instantiate(fireBallPrefeb, firePointRight.position, firePointRight.rotation);
        }
        else if (GetComponent<PlayerBehaviour>().jockDirection == 15)
        {
            Instantiate(fireBallPrefeb, firePointLeft.position, firePointLeft.rotation);
        }
        else if (GetComponent<PlayerBehaviour>().jockDirection == 5)
        {
            Instantiate(fireBallPrefeb, firePointUp.position, firePointUp.rotation);
        }
        else if (GetComponent<PlayerBehaviour>().jockDirection == 0)
        {
            Instantiate(fireBallPrefeb, firePointDown.position, firePointDown.rotation);
        }


    }
}
