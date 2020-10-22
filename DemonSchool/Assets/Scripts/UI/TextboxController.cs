using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextboxController : MonoBehaviour
{
    public static TextboxController instance;
    public static TextMeshProUGUI textboxText;

    public static Image characterImage;

    void Awake()
    {
        instance = this;
        characterImage = GameObject.FindGameObjectWithTag("CharImg").GetComponent<Image>();
        textboxText = GameObject.FindGameObjectWithTag("Textbox").GetComponent<TextMeshProUGUI>();
    }

    public static void UpdateText(string txt)
    {
        textboxText.text = txt;
    }

    public static void UpdateTextAndImage(string txt, Sprite charSpr)
    {
        textboxText.text = txt;
        characterImage.sprite = charSpr;
    }
}
