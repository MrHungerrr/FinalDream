using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{

   [HideInInspector]
   public bool power = false;
   [HideInInspector]
   private bool blackout = false;

   public EnemyHelperAI enemyHelpAI;
   private float distBuf;
   private float dist;
   private float blackoutDist;
   private float overloadDist;


   public Renderer lamp;
   private Material material;
   public Light pointLight;

   public Color colorOn = new Color(1, 1, 1);
   private Color colorOff = new Color(0, 0, 0);



   void Start()
   {
      blackoutDist = enemyHelpAI.blackoutDist;
      overloadDist = enemyHelpAI.overloadDist;

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
      if (enemyHelpAI.night)
      {
         dist = blackoutDist + 1;

         for (int i = 0; i < enemyHelpAI.orblessCount; i++)
         {
            distBuf = (transform.position - enemyHelpAI.orblesses[i].transform.position).magnitude;
            if (dist > distBuf)
               dist = distBuf;
         }

            if (dist <= overloadDist)
         {
            blackout = false;
            PowerOn((1 / ((dist - 0.5f))) + Random.Range(-0.1f, 0.1f));
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
               PowerOff();
               this.tag = "electricityOff";
               blackout = false;
            }
            else
            {
               Power();
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
      material.SetColor("_EmissionColor", colorOn * intensivity);
      pointLight.intensity = Mathf.Lerp(pointLight.intensity, 1.5f * intensivity, 0.3f);

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
