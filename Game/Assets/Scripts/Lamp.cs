using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{

   [HideInInspector]
   public bool power = false;
   [HideInInspector]
   public bool danger = false;
   private bool blackout = false;

   public Transform[] orbless = new Transform[3];
   private float distBuf;
   private float dist;
   private float blackoutDist = 75;
   private float overloadDist = 3;


   public Renderer lamp;
   private Material material;
   public Light pointLight;

   public Color colorOn = new Color(1, 1, 1);
   private Color colorOff = new Color(0, 0, 0);



   void Start()
   {
      material = lamp.GetComponent<Renderer>().material;
      PowerOff();

      if (this.tag == "electricityOn")
      {
         power = true;
         PowerOn();
      }

      if (this.tag == "electricityOff")
      {
         power = false;
         PowerOff();
      }


   }



   void Update()
   {
      if (danger)
      {
         dist = 0;

         for (int i = 0; i < orbless.Length; i++)
         {
            distBuf = (transform.position - orbless[i].position).magnitude;
            if (dist < distBuf)
               dist = distBuf;
         }

         if (dist <= overloadDist)
         {
            PowerOn(1 / Mathf.Sqrt(dist));
         }
         else if (dist <= blackoutDist)
         {
            if (!blackout)
            {
               PowerOff();
               blackout = true;
            }
         }
         else
         {
            if (blackout)
            {
               Power();
               blackout = false;
            }
         }
      }
      else
      {
         Power();
      }

   }



   private void Power()
   {
      if (this.tag == "electricityOn" && !power)
      {
         PowerOn();
      }

      if (this.tag == "electricityOff" && power)
      {
         PowerOff();
      }
   }



   private void PowerOn()
   {
      power = true;
      material.SetColor("_EmissionColor", colorOn * 3);
      pointLight.intensity = 1.5f;

   }



   private void PowerOn(float intensivity)
   {
      power = true;
      material.SetColor("_EmissionColor", colorOn *intensivity);
      pointLight.intensity = 1.5f * intensivity;

   }



   private void Blackout()
   {

   }



   private void PowerOff()
   {
      power = false;
      material.SetColor("_EmissionColor", colorOff);
      pointLight.intensity = 0f;
   }
}
