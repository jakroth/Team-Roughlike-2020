using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NoteDictionary : MonoBehaviour
{
    public List<string> notes;
    private static List<string> staticNotes = new List<string>();

    void Awake()
    {
        foreach(string str in notes)
        {
            staticNotes.Add(str);
        }
    }

    // Update is called once per frame
    public static string RandomNote()
    {
        return staticNotes[UnityEngine.Random.Range(0, staticNotes.Count)];
    }
}
