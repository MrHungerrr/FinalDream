using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{

   public PlayerScript player;
   public EnemyHelperAI eHelpAI;
   public GlitchEffect glitch;
   private float distBuf;
   [HideInInspector]
   public float dist;
   private float blackoutDist;
   private float overloadDist;
   private float sonarOffDist = 17f;
   private bool blackout = false;
   private float timeCD;
   private const float timeCD_N = 5f;
   private float orblessGlitch;
   private float GlitchCD;
   private const float GlitchCD_N =1f;

   [FMODUnity.EventRef]
   public string sonarSound;
   private float time = 1;

   [ContextMenu("AutoFill")]
   public void Fiil()
   {
      eHelpAI = GameObject.Find("EnemyHelper").GetComponent<EnemyHelperAI>();
      player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
      glitch = GameObject.Find("Main Camera").GetComponent<GlitchEffect>();
   }


   void Start()
   {
      blackoutDist = eHelpAI.blackoutDist;
      overloadDist = eHelpAI.overloadDist;
      dist = blackoutDist + 1;
      glitch.intensity = 0;
      glitch.flipIntensity = 0;
      glitch.colorIntensity = 0;
   }

   // Update is called once per frame
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
            //Debug.Log("Перенапряжение");
            player.suitOff = false;
            Light(((1 / dist) + Random.Range(-orblessGlitch, orblessGlitch))*2);
            if (GlitchCD <= 0)
            {
               glitch.intensity = 1 / dist;
               glitch.flipIntensity = 1 / dist;
               glitch.colorIntensity = 1 / dist;
               GlitchCD = GlitchCD_N;
            }
            else
            {
               GlitchCD -= Time.deltaTime;
            }
            blackout = false;

         }
         else if (dist <= blackoutDist)
         {
            Distance();
            if (!blackout)
            {
               glitch.intensity = 0;
               glitch.flipIntensity = 0;
               glitch.colorIntensity = 0;
               //Debug.Log("Отрубается костюм");
               player.suitOff = true;
               Light(0);
               blackout = true;
            }
         }
         else
         {
            if (blackout)
            {
               //Debug.Log("Нормальная работа");
               player.suitOff = false;
               Light(1);
               timeCD = 0;
               blackout = false;
            }
         }
      }
   }



   private void Light(float coef)
   { 

      player.lights_suit[0].intensity = player.lights_suit_intens[0] * (coef + 0.5f);
      player.lights_suit[1].intensity = player.lights_suit_intens[1] * coef;
      player.mana_intensity = 6 * coef;
      player.hp_intensity = 1 * coef;
      player.LightSuit();
   }



   private void Distance()
   {
      if (dist > sonarOffDist)
      {
         if (timeCD<=0)
         {
            if (time > 0)
            {
               time -= Time.deltaTime;
            }
            else
            {
               FMODUnity.RuntimeManager.PlayOneShot(sonarSound);
               time = (dist / (blackoutDist-30));
               time *= time*time;
            }
         }
         else
         {
            timeCD -= Time.deltaTime;
         }

      }
      else
      {
         timeCD = timeCD_N;
      }
   }
}
