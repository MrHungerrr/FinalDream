using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTransparency : MonoBehaviour
{

   public Renderer[] walls;
   public Collider[] enter;
   public Collider[] exit;
   public bool inDoor;
   private Color[] colors;
   private int wallsCount;
   private int enterCount;
   private int exitCount;
   public bool transparency;
   private bool end = false;
   private float transValue;
   public PlayerScript pScript;


   [ContextMenu("AutoFill")]
   public void Fill()
   {
      pScript = GameObject.Find("Suit").GetComponent<PlayerScript>();
   }



   private void Start()
   {
      wallsCount = walls.Length;
      enterCount = enter.Length;
      exitCount = exit.Length;
      colors = new Color[wallsCount];
      for(int i = 0; i<wallsCount; i++)
      {
         colors[i] = walls[i].material.color;
      }

   }



   private void OnTriggerEnter(Collider obj)
   {
      if (obj.tag == "Player")
      {
         transparency = !transparency;
         end = false;
      }
   }


   void Update()
   {
      if (!end)
      {
         if (transparency)
         {
            transValue = Mathf.Lerp(transValue, 0.0f, 0.2f);

            for (int i = 0; i < enterCount; i++)
               enter[i].enabled = false;
            for (int i = 0; i < exitCount; i++)
               exit[i].enabled = true;

            if (inDoor && !pScript.suitOff)
            { 
               pScript.lights_suit[0].intensity =(pScript.lights_suit_intens[0] + 0.2f * (1-transValue));
            }
            
         }
         else
         {
            transValue = Mathf.Lerp(transValue, 1.01f, 0.2f);

            for (int i = 0; i < enterCount; i++)
               enter[i].enabled = true;
            for (int i = 0; i < exitCount; i++)
               exit[i].enabled = false;

            if (inDoor && !pScript.suitOff)
            {
               pScript.lights_suit[0].intensity = (pScript.lights_suit_intens[0] + 0.2f * (1 - transValue));
            }
         }



         for (int i = 0; i < wallsCount; i++)
         {
            walls[i].material.color = new Color(colors[i].r, colors[i].g, colors[i].b, transValue);
         }

         if (transValue == 0.0f || transValue >=1.0f)
         {
            end = true;
         }
      }

      
   }
}
