using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTransparency : MonoBehaviour
{

   public Renderer[] walls = new Renderer[2];
   private Color[] colors = new Color[2];
   private int wallsCount;
   private bool transparency;
   private bool end;
   private float transValue;

   private void Start()
   {
      wallsCount = walls.Length;
      for(int i = 0; i<wallsCount; i++)
      {
         colors[i] = walls[i].material.color;
      }
   }



   private void OnTriggerEnter(Collider obj)
   {
      if (obj.tag == "Player")
      {
         transparency = true;
         end = false;
      }
   }



   private void OnTriggerExit(Collider obj)
   {
      if (obj.gameObject.tag == "Player")
      {
         transparency = false;
         end = false;
      }
   }



   void Update()
   {
      if(!end)
      {
         if(transparency)
         {
            transValue = Mathf.Lerp(transValue, 0.0f, 0.2f);
         }
         else
         {
            transValue = Mathf.Lerp(transValue, 1.1f, 0.2f);
         }

         for (int i = 0; i < wallsCount; i++)
         {
            walls[i].material.color = new Color(colors[i].r, colors[i].g, colors[i].b, transValue);
         }


         if (transValue == 0.0f || transValue > 1.0f)
            end = true;
      }
   }
}
