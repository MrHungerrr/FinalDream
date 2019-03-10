using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{
   public AudioSource sonar;
   public AudioSource sonarOff;
   public PlayerScript player;

   private float time;
   private float time_N;
   private float timeCD;
   private float timeCD_N = 5;
   private bool sonarAct = false;
   private bool sonarOut = false;
   private bool sonarDisable = false;

   private float alarmDistMax = 75;
   private float alarmDistMin = 15;

    void Start()
    {
        
    }

   // Update is called once per frame
   void Update()
   {
      if (sonarAct)
      {
         if (!sonarDisable)
         {
            if (time > 0)
               time -= Time.deltaTime;
            else
            {
               sonar.Play();

               time = time_N;
            }
         }
         else
         {
            if (timeCD > 0)
               timeCD -= Time.deltaTime;
            else
            {
               sonarDisable = false;
               for (int i = 0; i < player.lights_suit.Length; i++)
               {
                  player.lights_suit[i].intensity = player.lights_suit_intens[i];
               }

               player.sonarDis = false;
               player.mana_intensity = 6;
               player.LightSuit();
            }
         }
      }

      if(sonarOut)
      {
         if (!sonarDisable)
         {
            sonar.Play();
            sonarOff.Play();
            sonarDisable = true;
            timeCD = timeCD_N;
            player.sonarDis = true;

            for (int i = 0; i < player.lights_suit.Length; i++)
            {
               player.lights_suit[i].intensity = 0;
            }

            player.mana_intensity = 0;
            player.LightSuit();
         }
      }
   }

   public void Distance(float dist)
   {
      if (dist < alarmDistMax && dist > alarmDistMin)
      {
         sonarAct = true;
         time_N = (dist / (alarmDistMax-25)) * (dist / (alarmDistMax-25));
      }
      else
      {
         sonarAct = false;
      }
      
      if (dist<alarmDistMin)
      {
         sonarOut = true;
      }
      else
      {
         sonarOut = false;
      }
   }
}
