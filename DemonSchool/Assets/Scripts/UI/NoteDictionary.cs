using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NoteDictionary : MonoBehaviour
{
    public List<string> backpackNotes;
    public List<string> studentRescues;
    private static List<string> staticNotes = new List<string>();
    private static List<string> staticRescues = new List<string>();

    void Awake()
    {
        foreach(string str in backpackNotes)
            staticNotes.Add(str);
        foreach(string str in studentRescues)
            staticRescues.Add(str);
    }
  
    public static string RandomNote()
    {
        return staticNotes[UnityEngine.Random.Range(0, staticNotes.Count)];
    }

    
    public static string StudentRescue()
    {
        return staticRescues[UnityEngine.Random.Range(0, staticRescues.Count)];
    }
   
}
