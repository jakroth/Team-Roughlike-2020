using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSoundManager : MonoBehaviour
{

    [Serializable] 
    public struct Sounds {
        public string name;
        public AudioClip audioClip;
    }

    public List<Sounds> soundsList = new List<Sounds>();

    private bool isWalking = false;
    private bool playNewFootstep = true;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playNewFootstep && isWalking)
        {
            StartCoroutine(LoopFootstepSounds());
            playNewFootstep = false;
        }
    }

    private IEnumerator LoopFootstepSounds()
    {

        int footstep = UnityEngine.Random.Range(0, 5);
        audioSource.PlayOneShot(soundsList[footstep].audioClip);
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 0.5f));
        playNewFootstep = true;
    }

    public void PlayFootsteps()
    {
        isWalking = true;
        //StartCoroutine(LoopFootstepSounds());
    }

    public void EndFootsteps()
    {
        isWalking = false;
        audioSource.Stop();
    }
}
