using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

   private bool pause = false;

   public GameObject player;
   private PlayerScript pScript;

   [ContextMenu("AutoFill")]
   public void Fill()
   {
      player = GameObject.Find("Suit");
   }



   private void Start()
   {
      pScript = player.GetComponent<PlayerScript>();
   }



   void Update()
   {

   }
}
