using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Сonductor: MonoBehaviour
{
   public GameObject[] mechanism;
   private int temp;
   private float forceTime;
   private const float forceTime_N = 2f;
   private bool change;


   [HideInInspector]
   public bool power = false;
   [HideInInspector]
   private bool blackout = false;
   private EnemyHelperAI eHelpAI;
   private float distBuf;
   private float dist;
   private float blackoutDist;
   private float overloadDist;
   private float orblessGlitch;


   public Renderer emissionLamp;
   private float lightIntens;
   private Material material;
   public Light lightTrue;
   private bool act;
   private Animator anim;

   private Color colorOn = new Color(0, 1, 0);
   private Color colorOff = new Color(0, 0, 0);
   private Color colorIce = new Color(0, 0.5f, 1.0f);
   private Color colorFire = new Color(1.0f, 0.5f, 0);

   private Color colorLightOn = new Color(0.4f, 1, 0.4f);
   private Color colorLightOff = new Color(0, 0, 0);
   private Color colorLightIce = new Color(0.5f, 0.75f, 1.0f);
   private Color colorLightFire = new Color(1.0f, 0.7f, 0.3f);


   void Start()
   {
      eHelpAI = GameObject.Find("EnemyHelper").GetComponent<EnemyHelperAI>();
      material = emissionLamp.material;
      //anim = GetComponent<Animator>();
      lightIntens = lightTrue.intensity;
      blackoutDist = eHelpAI.blackoutDist;
      overloadDist = eHelpAI.overloadDist;

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
            PowerOn(((lightIntens / dist) + Random.Range(-orblessGlitch, orblessGlitch)) * 2);
         }
         else if (dist <= blackoutDist)
         {
            if (!blackout)
            {
               blackout = true;
               PowerOff();
            }
         }
         else
         {
            if (blackout)
            {
               blackout = false;
               power = false;
               PowerOff();
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
      if (this.tag == "electricityOn")
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
      if (change)
      {
         power = true;
         switch (temp)
         {
            case -1:
               {
                  //anim.SetBool("Active", false);
                  material.SetColor("_EmissionColor", colorIce * 5);
                  lightTrue.color = colorLightIce;
                  change = false;
                  if (mechanism.Length > 0)
                     for (int i = 0; i < mechanism.Length; i++)
                     {
                        mechanism[i].tag = "electricityOff";
                     }
                  break;
               }
            case 0:
               {
                  //anim.SetBool("Active", true);
                  material.SetColor("_EmissionColor", colorOn * 5);
                  lightTrue.color = colorLightOn;
                  change = false;
                  if (mechanism.Length > 0)
                     for (int i = 0; i < mechanism.Length; i++)
                     {
                        mechanism[i].tag = "electricityOn";
                     }
                  break;
               }
            case 1:
               {
                  //anim.SetBool("Active", false);
                  material.SetColor("_EmissionColor", colorFire * 5);
                  lightTrue.color = colorLightFire;
                  change = false;
                  if (mechanism.Length > 0)
                     for (int i = 0; i < mechanism.Length; i++)
                     {
                        mechanism[i].tag = "electricityOff";
                     }
                  break;
               }
         }
      }
   }



   private void PowerOn(float intesivity)
   {
      //anim.SetBool("Active", true);
      material.SetColor("_EmissionColor", colorOn * 5 * intesivity);
      lightTrue.color = colorLightOn;
      lightTrue.intensity = Mathf.Lerp(lightTrue.intensity, intesivity, 0.3f);
   }



   private void PowerOff()
   {
      //anim.SetBool("Active", false);
      power = false;
      change = true;
      lightTrue.color = colorLightOff;
      material.SetColor("_EmissionColor", colorOff);
      for (int i = 0; i < mechanism.Length; i++)
      {
         mechanism[i].tag = "electricityOff";
      }
   }



   private void OnTriggerStay(Collider force)
   {
      act = true;

      if (force.tag == "ice" && temp > -1)
      {
         forceTime -= Time.fixedDeltaTime;
         if (forceTime <= -forceTime_N)
         {
            change = true;
            temp--;
            forceTime = 0;
         }
      }

      if (force.tag == "fire" && temp < 1)
      {
         forceTime += Time.fixedDeltaTime;
         if (forceTime >= forceTime_N)
         {
            change = true;
            temp++;
            forceTime = 0;
         }
      }


   }


}
