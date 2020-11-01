using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCollectable : MonoBehaviour
{
    public TutorialManager tutorialManager;

    public string type = "";

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            switch(type)
            {
                case "holywater":
                    if(tutorialManager.part == 1)
                    {
                        tutorialManager.StartAttackDialogue();
                        Destroy(this.gameObject);
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
                default:
                break;
            }

        }
    }

}
