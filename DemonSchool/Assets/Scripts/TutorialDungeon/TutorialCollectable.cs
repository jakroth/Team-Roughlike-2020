﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCollectable : MonoBehaviour
{
    public TutorialManager tutorialManager;
    [SerializeField] private SceneLoader sceneLoader = null;

    public string type = "";

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player" || col.gameObject.tag == "FakePlayer")
        {
            switch(type)
            {
                case "holywater":
                    if(tutorialManager.part == 1)
                    {
                        tutorialManager.StartAttackDialogue();
                    }
                    break;
                case "door":
                    if(tutorialManager.part == 2)
                        tutorialManager.StartDoorDialogue(this.gameObject);
                    break;
                case "spider":
                    if(tutorialManager.part == 3)
                    {
                        tutorialManager.StartSpiderDialogue();
                        Destroy(this.gameObject);
                    }
                    break;
                case "vanish":
                    col.gameObject.GetComponent<FadeController>().FadeOut();
                    Destroy(col.gameObject, 0.5f);
                    StartCoroutine(LoadTheScene());
                    break;
                case "final":
                    tutorialManager.StartFinalTransition();
                    Destroy(this.gameObject);
                    break;
                default:
                break;
            }

        }
    }

    void Awake()
    {
        sceneLoader = GameController.instance.gameObject.GetComponent<SceneLoader>();
    }

    private IEnumerator LoadTheScene()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject.FindGameObjectWithTag("manager").GetComponent<FadeController>().FadeInAndOut(2f);
        sceneLoader.LoadNextScene();
    }

}
