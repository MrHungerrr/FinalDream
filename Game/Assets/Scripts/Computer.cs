using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour
{
   public Door[] doors;
   [HideInInspector]
   public bool power = false;
   private bool powerNow = false;
   [HideInInspector]
   private bool blackout = false;
   public bool doorLock;

   private EnemyHelperAI eHelpAI;
   private float distBuf;
   private float dist;
   private float blackoutDist;
   private float overloadDist;
   private float lightIntens;
   private float orblessGlitch;

   public Renderer monitor;
   public Renderer lockLamp;
   private Material monitor_mat;
   private Material lockLamp_mat;
   public Light lightTrue;

   [SerializeField]
   private Color colorOn;
   [SerializeField]
   private float emisIntens;
   private Color colorOff = new Color(0, 0, 0);
   private Color colorLock = new Color(1, 0, 0);
   private Color colorUnLock = new Color(0, 1, 0);



   void Start()
   {
      eHelpAI = GameObject.Find("EnemyHelper").GetComponent<EnemyHelperAI>();
      lightIntens = lightTrue.intensity;
      blackoutDist = eHelpAI.blackoutDist;
      overloadDist = eHelpAI.overloadDist;

      monitor_mat = monitor.material;
      lockLamp_mat = lockLamp.material;
      lightTrue.color = colorOn;

      this.tag = "computer";

      if (power)
      {
         PowerOn();
      }
      else
      {
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
               PowerOff();
               blackout = true;
            }
         }
         else
         {
            if (blackout)
            {
               PowerOff();
               power = false;
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
      if (power && !powerNow)
      {
         PowerOn();
      }

      if (!power && powerNow)
      {
         PowerOff();
      }
   }

   private void PowerOn()
   {
      powerNow = true;
      monitor_mat.SetColor("_EmissionColor", colorOn * emisIntens);
      lightTrue.color = colorOn;
      lightTrue.intensity = lightIntens;
      if(doorLock)
      {
         lockLamp_mat.SetColor("_EmissionColor", colorLock);
      }
      else
      {
         lockLamp_mat.SetColor("_EmissionColor", colorUnLock);
      }
   }


   public void Unlock()
   {
      doorLock = !doorLock;
      if (doorLock)
      {
         for (int i = 0; i < doors.Length; i++)
         {
            doors[i].lockDoor = true;
            if(doors[i].power)
               doors[i].PowerOn();
         }
      }
      else
      {
         for (int i = 0; i < doors.Length; i++)
         {
            doors[i].lockDoor = false;
            if (doors[i].power)
               doors[i].PowerOn();
         }
      }

      PowerOn();
   }


   private void PowerOn(float intensivity)
   {
      monitor_mat.SetColor("_EmissionColor", colorOn * emisIntens * intensivity);
      lockLamp_mat.SetColor("_EmissionColor", colorUnLock * intensivity);
      lightTrue.color = colorOn;
      lightTrue.intensity = Mathf.Lerp(lightTrue.intensity, 1.5f * intensivity, 0.3f);

   }


   private void PowerOff()
   {
      powerNow = false;
      monitor_mat.SetColor("_EmissionColor", colorOff);
      lockLamp_mat.SetColor("_EmissionColor", colorOff);
      lightTrue.intensity = 0f;
   }
}
