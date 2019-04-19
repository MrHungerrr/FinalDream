using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : MonoBehaviour
{
   private bool power = false;
   public Generator generator;
   private bool active;
   public byte nomber;

   private bool blackout = false;
   private EnemyHelperAI eHelpAI;
   private float distBuf;
   private float dist;
   private float blackoutDist;
   private float overloadDist;
   private float orblessGlitch;

   public Renderer[] emissionLamps;
   private Material[] materials;
   private int lampsQuant;
   public Light lightTrue;
   private float lightIntens;
   private Animator anim;

   private Color colorOnLamps = new Color(1f, 1f, 0f);
   private Color colorOn = new Color(0, 1, 0);
   private Color colorOff = new Color(1, 0, 0);
   private Color colorBlackout = new Color(0, 0, 0);
   private FMOD.Studio.EventInstance soundEngine;

   void Awake()
   {

      soundEngine = FMODUnity.RuntimeManager.CreateInstance("event:/Engine");
      eHelpAI = GameObject.Find("EnemyHelper").GetComponent<EnemyHelperAI>();
      lampsQuant = emissionLamps.Length;
      materials = new Material[lampsQuant];
      blackoutDist = eHelpAI.blackoutDist;
      overloadDist = eHelpAI.overloadDist;
      materials[0] = emissionLamps[0].material;
      materials[1] = emissionLamps[1].material;
      soundEngine.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
      lightIntens = lightTrue.intensity;

      for (int i = 0; i < lampsQuant; i++)
      {
         emissionLamps[i].material = materials[i];
      }

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
            PowerOn(((1 / dist) + Random.Range(-orblessGlitch, orblessGlitch)) * 2);
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
               PowerOff();
            }
            else
            {
               Power();
            }
         }

      }
      else if (blackout)
      {
         blackout = false;
         power = false;
         PowerOff();
      }
      else
         Power();
   }

   public void Switch()
   {
      active = !active;
      PowerOn();
   }

   private void Power()
   {
      if (!power && this.tag == "electricityOn")
      {
         PowerOn();
      }

      if (power && this.tag == "electricityOff")
      {
         PowerOff();
      }
   }

   private void PowerOn()
   {
      power = true;
      lightTrue.intensity = lightIntens;
      materials[0].SetColor("_EmissionColor", colorOnLamps * 2.5f);
      soundEngine.start();
      this.tag = "terminal";

      if (active)
      {
         materials[1].SetColor("_EmissionColor", colorOn * 2.5f);
         generator.TurnOn(nomber);
      }
      else
      {
         materials[1].SetColor("_EmissionColor", colorOff * 2.5f);
         generator.TurnOff(nomber);
      }
   }

   private void PowerOn(float intesivity)
   {
      soundEngine.start();
      materials[0].SetColor("_EmissionColor", colorOnLamps * 2.5f * intesivity);
      materials[1].SetColor("_EmissionColor", colorOn * 2.5f * intesivity);
      lightTrue.intensity = Mathf.Lerp(lightTrue.intensity, lightIntens * intesivity, 0.3f);
   }



   private void PowerOff()
   {
      if (!blackout)
      {
         power = false;
         soundEngine.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
         lightTrue.intensity = 0;
         materials[0].SetColor("_EmissionColor", colorBlackout);
         materials[1].SetColor("_EmissionColor", colorBlackout);
      }
   }
}
