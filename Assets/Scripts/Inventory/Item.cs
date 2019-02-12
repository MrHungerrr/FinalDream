using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

 
    public string sprite;
    public string prefab;
    public string Name;
    public string Description;
    public string Type;
    public bool iscollected = false;
    public DateTime timecollected;

    private void Start()
    {
        if (gameObject.name[0] == 'A')
            Type = "Audio";
        else if (gameObject.name[0] == 'N')
        {
            Type = "Note";
        }
            
    }

}
