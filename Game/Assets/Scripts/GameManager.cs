using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

   private GameObject player;
   private PlayerScript pScript;


   private void Start()
   {
      player = GameObject.FindGameObjectWithTag("Player");
      pScript = player.GetComponent<PlayerScript>();
   }



   void Update()
   {

   }
}
