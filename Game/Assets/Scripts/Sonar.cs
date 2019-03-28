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
   private float sonarOffDist = 10;
   private bool blackout = false;

   [FMODUnity.EventRef]
   public string sonarSound;
   private float time = 0;

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
            Light((1 / (dist - 0.5f) + Random.Range(-0.1f, 0.1f)));
            blackout = false;
         }
         else if (dist <= blackoutDist)
         {
            if (!blackout)
            {
               //Debug.Log("Отрубается костюм");
               player.suitOff = true;
               Light(0);
               Distance();
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
               blackout = false;
            }
         }
      }
   }



   private void Light(float coef)
   { 
      for (int i = 0; i < player.lights_suit.Length; i++)
      {
         player.lights_suit[i].intensity = player.lights_suit_intens[i] * coef;
      }

      player.mana_intensity = 6 * coef;
      player.hp_intensity = 1 * coef;
      player.LightSuit();
   }



   private void Distance()
   {
      if (dist > sonarOffDist)
      {
         if (time > 0)
         {
            time -= Time.deltaTime;
         }
         else
         {
            FMODUnity.RuntimeManager.PlayOneShot(sonarSound);
            time = (dist / blackoutDist - sonarOffDist);
            time *= time;
         }
      }
   }
}
