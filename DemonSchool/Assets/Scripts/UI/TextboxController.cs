using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextboxController : MonoBehaviour
{
    public static TextboxController instance;
    public static TextMeshProUGUI textboxText;

    void Awake()
    {
        instance = this;
        textboxText = GameObject.FindGameObjectWithTag("Textbox").GetComponent<TextMeshProUGUI>();
    }

    public static void UpdateText(string txt)
    {
        textboxText.text = txt;
    }
}
