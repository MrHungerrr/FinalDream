using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{
   public AudioSource sonar;
   public AudioSource sonarOff;

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
               sonarDisable = false;
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
         Debug.Log(dist);
         sonarOut = true;
      }
      else
      {
         sonarOut = false;
      }
   }
}
