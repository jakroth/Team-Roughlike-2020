using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour
{

    private Image image;
    private PlayerBehaviour playerBehaviour;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        playerBehaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        float value = playerBehaviour.playerHealth / 100f;
        //if(value > 0.99f)
        //    value = 1f;
        image.fillAmount = value;
        image.color = Color.Lerp(Color.red, Color.green, value);
    }
}
