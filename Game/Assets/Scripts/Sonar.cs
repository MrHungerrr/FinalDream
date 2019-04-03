using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{

   public PlayerScript player;
   public EnemyHelperAI eHelpAI;
   private float distBuf;
   [HideInInspector]
   public float dist;
   private float blackoutDist;
   private float overloadDist;
   private float sonarOffDist = 17f;
   private bool blackout = false;
   private float timeCD;
   private const float timeCD_N = 5f;

   [FMODUnity.EventRef]
   public string sonarSound;
   private float time = 1;

   [ContextMenu("AutoFill")]
   public void Fiil()
   {
      eHelpAI = GameObject.FindGameObjectWithTag("enemyHelper").GetComponent<EnemyHelperAI>();
      player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
   }

   void Start()
   {
      blackoutDist = eHelpAI.blackoutDist;
      overloadDist = eHelpAI.overloadDist;
      dist = blackoutDist + 1;
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
               dist = distBuf;
         }

         if (dist <= overloadDist)
         {
            //Debug.Log("Перенапряжение");
            player.suitOff = false;
            Light(((1 / dist) + Random.Range(-0.1f, 0.1f))*2);
            blackout = false;
         }
         else if (dist <= blackoutDist)
         {
            Distance();
            if (!blackout)
            {
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
      Debug.Log(dist);
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
               Debug.Log("Fuck");
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
