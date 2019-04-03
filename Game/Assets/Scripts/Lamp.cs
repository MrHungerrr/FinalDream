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
   private float lightIntens;


   public Renderer[] lamps;
   private Material[] materials;
   public Light trueLight;

   private Color[] colorOn;
   private Color colorOff = new Color(0, 0, 0);



   void Start()
   {
      lightIntens = trueLight.intensity;
      blackoutDist = enemyHelpAI.blackoutDist;
      overloadDist = enemyHelpAI.overloadDist;
      materials = new Material[lamps.Length];
      colorOn = new Color[lamps.Length];

      for (int i = 0; i < lamps.Length; i++)
      {
         materials[i] = lamps[i].GetComponent<Renderer>().material;
         colorOn[i] = materials[i].color;
      }

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
            PowerOn(((lightIntens / dist) + Random.Range(-0.1f, 0.1f))*2);
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
      for (int i = 0; i < lamps.Length; i++)
         materials[i].SetColor("_EmissionColor", colorOn[i] * 3);
      trueLight.intensity = lightIntens;

   }


   private void PowerOn(float intensivity)
   {
      power = true;
      for (int i = 0; i < lamps.Length; i++)
         materials[i].SetColor("_EmissionColor", colorOn[i] * 3 * intensivity);
      trueLight.intensity = Mathf.Lerp(trueLight.intensity, 1.5f * intensivity, 0.3f);

   }


   private void Blackout()
   {

   }


   private void PowerOff()
   {
      power = false;
      for (int i = 0; i < lamps.Length; i++)
         materials[i].SetColor("_EmissionColor", colorOff * 3);
      trueLight.intensity = 0f;
   }
}
