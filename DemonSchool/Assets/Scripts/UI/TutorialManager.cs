﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class TutorialManager : MonoBehaviour
{
    public List<Sprite> characters;
    public List<string> tutorialDialogue;
    private static List<Sprite> staticCharacters = new List<Sprite>();
    private static List<string> staticDialogue = new List<string>();

    private FadeController fadeController;

    private string currentText = null;
    private string nextText = null;

    private void Awake()
    {
        foreach(Sprite spr in characters)
            staticCharacters.Add(spr);
        foreach(string str in tutorialDialogue)
            staticDialogue.Add(str);
    }
    private void Start()
    {
        fadeController = GameObject.FindGameObjectWithTag("UI").GetComponent<FadeController>();
        currentText = "S: ugh… what happened?";
        TextboxController.UpdateTextAndImage("ugh… what happened?", staticCharacters[0]);
        StartCoroutine(BeginTutorial());
    }

    private IEnumerator BeginTutorial()
    {
        yield return new WaitForSeconds(1f);
        fadeController.FadeIn();
    }

    public void NextDialogue()
    {
        //check next dialogue based on current dialogue
        for(int i = 0; i < tutorialDialogue.Count + 1; i++)
        {
            if(tutorialDialogue[i] == currentText)
            {
                if(i >= 12)
                    nextText = null;
                else
                    nextText = tutorialDialogue[i + 1];
                break;
            }
        }
        Debug.Log(nextText);

        if(nextText != null)
        {
            currentText = nextText;
            //set next dialogue and image based on dialogue
            string str = nextText.Substring(0, 2);
            if(str == "S:")
            {
                nextText.Replace(str, string.Empty);
                TextboxController.UpdateTextAndImage(nextText, staticCharacters[0]);
            }
            else if(str == "N:")
            {
                nextText.Replace(str, string.Empty);
                TextboxController.UpdateTextAndImage(nextText, staticCharacters[1]);
            }
        }
        else
        {
            currentText = nextText;
            fadeController.FadeOut();
        }

    }
}
