using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTransparency : MonoBehaviour
{

   [Header("Wall Transparent")]
   public Renderer[] walls;
   public Collider[] enter;
   public Collider[] exit;
   public bool inDoor;
   private int wallsCount;
   private int enterCount;
   private int exitCount;
   public bool transparency;
   private bool end = false;
   private float transValue;
   private PlayerScript pScript;
   private LevelManager levMan;

   [Header("Nature Sound")]
   [Range(0f, 1f)]
   public float volume;
   [Range(10,22000)]
   public int cutOff;



   private void Start()
   {

      pScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
      levMan = GameObject.Find("GameManager").GetComponent<LevelManager>();
      wallsCount = walls.Length;
      enterCount = enter.Length;
      exitCount = exit.Length;
      if (transparency)
      {
         InOutDoor();
      }
      else
      {
         for (int i = 0; i < enterCount; i++)
            enter[i].enabled = true;
         for (int i = 0; i < exitCount; i++)
            exit[i].enabled = false;

         for (int i = 0; i < wallsCount; i++)
         {
            walls[i].enabled = true;
         }
      }
   }



   private void OnTriggerEnter(Collider obj)
   {
      if (obj.tag == "Player")
      {
         transparency = !transparency;
         InOutDoor();
      }
   }



   private void InOutDoor()
   {
      if (transparency)
      {
         for (int i = 0; i < enterCount; i++)
            enter[i].enabled = false;
         for (int i = 0; i < exitCount; i++)
            exit[i].enabled = true;

         for (int i = 0; i < wallsCount; i++)
         {
            walls[i].enabled = false;
         }

         if (inDoor)
         {
            levMan.NatureSound(cutOff, volume);
            pScript.lights_suit_intens[0] += 0.4f;
            if (!pScript.suitOff)
               pScript.lights_suit[0].intensity = pScript.lights_suit_intens[0];
         }
      }
      else
      {
         for (int i = 0; i < enterCount; i++)
            enter[i].enabled = true;
         for (int i = 0; i < exitCount; i++)
            exit[i].enabled = false;

         for (int i = 0; i < wallsCount; i++)
         {
            walls[i].enabled = true;
         }

         if (inDoor)
         {
            levMan.NatureSound(22000, 1);
            pScript.lights_suit_intens[0] -= 0.4f;
            if (!pScript.suitOff)
               pScript.lights_suit[0].intensity = pScript.lights_suit_intens[0];
         }
      }
   }
}
