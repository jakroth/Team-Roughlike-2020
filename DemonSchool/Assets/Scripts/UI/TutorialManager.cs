using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class TutorialManager : MonoBehaviour
{
    public List<Sprite> characters;
    public List<string> tutorialDialogue;
    public List<string> attackDialogue;
    public List<string> doorDialogue;
    public List<string> spiderDialogue;
    public List<string> deathDialogue;
    private static List<Sprite> staticCharacters = new List<Sprite>();
    private static List<string> staticDialogue = new List<string>();

    private FadeController fadeController;

    private string currentText = null;
    private string nextText = null;

    public int part = 0;

    [SerializeField] private bool isDoorDialogue = false;
    [SerializeField] private bool endDialogue = false;
    [SerializeField] private GameObject doorRef = null;
    [SerializeField] private GameObject baseScene = null;
    [SerializeField] private GameObject fallingScene = null;
    [SerializeField] private GameObject fakePlayer = null;

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
        GameController.instance.UpdatePauseState(true);
        fadeController.FadeIn();
    }

    public void StartAttackDialogue()
    {
        GameController.instance.UpdatePauseState(true);
        staticDialogue.Clear();
        foreach(string str in attackDialogue)
            staticDialogue.Add(str);
        currentText = "N: Don't drink that!";
        TextboxController.UpdateTextAndImage("Don't drink that!", staticCharacters[1]);
        StartCoroutine(BeginDialogue());
    }

    public void StartDoorDialogue(GameObject door)
    {
        GameController.instance.UpdatePauseState(true);
        staticDialogue.Clear();
        foreach(string str in doorDialogue)
            staticDialogue.Add(str);
        currentText = "N: Ok, I’m going to leave it up to you to work out what is going on out there.";
        TextboxController.UpdateTextAndImage("Ok, I’m going to leave it up to you to work out what is going on out there.", staticCharacters[1]);
        isDoorDialogue = true;
        doorRef = door;
        StartCoroutine(BeginDialogue());
    }

    public void StartSpiderDialogue()
    {
        GameController.instance.UpdatePauseState(true);
        staticDialogue.Clear();
        foreach(string str in spiderDialogue)
            staticDialogue.Add(str);
        currentText = "N: Steven, use the attack buttons I told you about to kill it. If it hits you your HP will go down";
        TextboxController.UpdateTextAndImage("Steven, use the attack buttons I told you about to kill it. If it hits you your HP will go down", staticCharacters[1]);
        StartCoroutine(BeginDialogue());
    }

    public void StartDeathDialogue()
    {
        GameController.instance.UpdatePauseState(true);
        staticDialogue.Clear();
        foreach(string str in deathDialogue)
            staticDialogue.Add(str);
        currentText = "N: In future, don’t be an idiot. Also do not hang up your phone, let me know what happens so I can help you out. Also my name is Noelle, remember it.";
        TextboxController.UpdateTextAndImage("In future, don’t be an idiot. Also do not hang up your phone, let me know what happens so I can help you out. Also my name is Noelle, remember it.", staticCharacters[1]);
        StartCoroutine(BeginDialogue());
    }

    public void StartFinalTransition()
    {
        endDialogue = true;
        TextboxController.UpdateTextAndImage("... \n AAAAAAA!!!", staticCharacters[0]);
        StartCoroutine(BeginFinalDialogue());
        GameController.instance.UpdatePauseState(true);
        GameObject.FindGameObjectWithTag("manager").GetComponent<FadeController>().FadeInAndOut(2f);
        StartCoroutine(ChangeScenes());
    }

    private IEnumerator ChangeScenes()
    {
        yield return new WaitForSeconds(2f);
        baseScene.SetActive(false);
        fallingScene.SetActive(true);
        fallingScene.GetComponentInChildren<TutorialCamera>().ShakeCamera(0.01f, 100000f);
        GameObject.FindGameObjectWithTag("Player").SetActive(false);
        fakePlayer.SetActive(true);
        yield return new WaitForSeconds(2f);
        fadeController.FadeOut();
    }
    
    private IEnumerator BeginFinalDialogue()
    {
        yield return new WaitForSeconds(1f);
        fadeController.FadeIn();
    }

    private IEnumerator BeginDialogue()
    {
        yield return new WaitForSeconds(0.25f);
        fadeController.FadeIn();
    }


    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space) && GameController.instance.GetPauseState() == true && !endDialogue)
        {
            NextDialogue();
        }
    }

    public void NextDialogue()
    {
        //check next dialogue based on current dialogue
        for(int i = 0; i < staticDialogue.Count + 1; i++)
        {
            if(staticDialogue[i] == currentText)
            {
                if(i >= staticDialogue.Count - 1)
                    nextText = null;
                else
                    nextText = staticDialogue[i + 1];
                break;
            }
        }
        //Debug.Log(nextText);

        if(nextText != null)
        {
            currentText = nextText;
            //set next dialogue and image based on dialogue
            string str = nextText.Substring(0, 2);
            string removed = "";
            if(str == "S:")
            {
                removed = nextText.Replace(str, string.Empty);
                TextboxController.UpdateTextAndImage(removed, staticCharacters[0]);
            }
            else if(str == "N:")
            {
                removed = nextText.Replace(str, string.Empty);
                TextboxController.UpdateTextAndImage(removed, staticCharacters[1]);
            }
        }
        else
        {
            if(!isDoorDialogue)
            {
                currentText = nextText;
                GameController.instance.UpdatePauseState(false);
                fadeController.FadeOut();
                part++;
            }
            else
            {
                currentText = nextText;
                Destroy(doorRef.GetComponent<FadeController>().imageToFade.gameObject);
                GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(16.6f, 9.6f, 0f);
                Destroy(doorRef);
                doorRef = null;
                isDoorDialogue = false;
                GameController.instance.UpdatePauseState(false);
                fadeController.FadeOut();
                part++;
            }

        }

    }
}
