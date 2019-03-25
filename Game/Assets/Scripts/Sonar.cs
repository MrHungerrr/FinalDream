using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{

   public PlayerScript player;
   public EnemyHelperAI enemyHelpAI;
   private float distBuf;
   private float dist;
   private float blackoutDist;
   private float overloadDist;
   private float sonarOffDist = 20;
   private bool blackout;


   private float time;
   private float time_N;
   private float timeCD;
   private float timeCD_N = 5;
   private bool sonarAct = false;
   private bool sonarOut = false;
   private bool sonarDisable = false;

   void Start()
   {
      blackoutDist = enemyHelpAI.blackoutDist;
      overloadDist = enemyHelpAI.overloadDist;
   }

   // Update is called once per frame
   void Update()
   {
      if (enemyHelpAI.night)
      {
         dist = blackoutDist + 1;

         for (int i = 0; i < enemyHelpAI.orblessCount; i++)
         {
            distBuf = (transform.position - enemyHelpAI.orbless[i].transform.position).magnitude;
            if (dist > distBuf)
               dist = distBuf;
         }

         if (dist <= overloadDist)
         {
            player.sonarDis = false;
            Light(1 / Mathf.Sqrt(dist) + Random.Range(0.0f, 0.2f) - 0.1f);
            blackout = false;
         }
         else if (dist <= blackoutDist)
         {
            if (!blackout)
            {
               player.sonarDis = true;
               Light(0);
               Distance();
               blackout = true;
            }
         }
         else
         {
            if (blackout)
            {
               player.sonarDis = false;
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
      player.LightSuit();
   }



   private void Distance()
   {
      if (dist > sonarOffDist)
      {
         time_N = (dist / blackoutDist - sonarOffDist);
         time_N *= time_N; ;
      }
   }
}
