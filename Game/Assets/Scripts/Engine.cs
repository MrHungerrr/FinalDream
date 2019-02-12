using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour {


   public bool power;


	void Start () {
      if (power)
         this.tag = "actionOn";
      else
         this.tag = "actionOff";
	}
	
	void Update () {
		
	}
}
