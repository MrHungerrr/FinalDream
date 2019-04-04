using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{

   [HideInInspector]
   public bool power = false;
   [HideInInspector]
   private bool blackout = false;

   private EnemyHelperAI eHelpAI;
   private float distBuf;
   private float dist;
   private float blackoutDist;
   private float overloadDist;
   private float lightIntens;
   private float orblessGlitch;

   public Renderer[] emissionLamps;
   private Material[] materials;
   public Light lightTrue;

   private Color[] colorOn;
   private Color colorOff = new Color(0, 0, 0);



   void Start()
   {
      eHelpAI = GameObject.Find("EnemyHelper").GetComponent<EnemyHelperAI>();
      lightIntens = lightTrue.intensity;
      blackoutDist = eHelpAI.blackoutDist;
      overloadDist = eHelpAI.overloadDist;
      materials = new Material[emissionLamps.Length];
      colorOn = new Color[emissionLamps.Length];

      for (int i = 0; i < emissionLamps.Length; i++)
      {
         materials[i] = emissionLamps[i].GetComponent<Renderer>().material;
         colorOn[i] = materials[i].GetColor("_EmissionColor");
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
      if (eHelpAI.night)
      {
         dist = blackoutDist + 1;

         for (int i = 0; i < eHelpAI.orblessCount; i++)
         {
            distBuf = (transform.position - eHelpAI.orblesses[i].transform.position).magnitude;
            if (dist > distBuf)
            {
               orblessGlitch = eHelpAI.orblessAI[i].orbGlitchInt;
               dist = distBuf;
            }
         }

            if (dist <= overloadDist)
         {
            blackout = false;
            PowerOn(((lightIntens / dist) + Random.Range(-orblessGlitch, orblessGlitch))*2);
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
      for (int i = 0; i < emissionLamps.Length; i++)
         materials[i].SetColor("_EmissionColor", colorOn[i]);
      lightTrue.intensity = lightIntens;

   }


   private void PowerOn(float intensivity)
   {
      power = true;
      for (int i = 0; i < emissionLamps.Length; i++)
         materials[i].SetColor("_EmissionColor", colorOn[i] * intensivity);
      lightTrue.intensity = Mathf.Lerp(lightTrue.intensity, 1.5f * intensivity, 0.3f);

   }


   private void Blackout()
   {

   }


   private void PowerOff()
   {
      power = false;
      for (int i = 0; i < emissionLamps.Length; i++)
         materials[i].SetColor("_EmissionColor", colorOff);
      lightTrue.intensity = 0f;
   }
}
