using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{

   public bool power;
   private bool powerNow;
   public GameObject[] mechanism = new GameObject[0];


   [HideInInspector]
   private bool blackout = false;
   private EnemyHelperAI eHelpAI;
   private float distBuf;
   private float dist;
   private float blackoutDist;
   private float overloadDist;
   private float orblessGlitch;


   public Renderer[] emissionLamps;
   private Material material;
   private int lampsQuant;
   public Light lightTrue;
   private Animator anim;

   private Color colorOn = new Color(0, 1, 0);
   private Color colorOff = new Color(1, 0, 0);
   private Color colorBlackout = new Color(0, 0, 0);

   private Color colorLightOn = new Color(0.4f, 1, 0.4f);
   private Color colorLightOff = new Color(1, 0.35f, 0);
   private Color colorLightBlackout = new Color(0, 0, 0);


   void Awake()
   {
      eHelpAI = GameObject.Find("EnemyHelper").GetComponent<EnemyHelperAI>();
      lampsQuant = emissionLamps.Length;
      blackoutDist = eHelpAI.blackoutDist;
      overloadDist = eHelpAI.overloadDist;
      material = emissionLamps[0].material;

      for (int i = 0; i < lampsQuant; i++)
      {
         emissionLamps[i].material = material;
      }

      anim = GetComponent<Animator>();
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
      anim.SetBool("Active", true);
      lightTrue.color = colorLightOn;

      material.SetColor("_EmissionColor", colorOn * 5);


      if (mechanism.Length > 0)
         for (int i = 0; i < mechanism.Length; i++)
         {
            mechanism[i].tag = "electricityOn";
         }
   }

   private void PowerOn(float intesivity)
   {
      anim.SetBool("Active", true);
      material.SetColor("_EmissionColor", colorOn * 5 * intesivity);
      lightTrue.color = colorLightOn;
      lightTrue.intensity = Mathf.Lerp(lightTrue.intensity, intesivity, 0.3f);
   }



   private void PowerOff()
   {
      if (!blackout)
      {
         powerNow = false;
         anim.SetBool("Active", false);
         lightTrue.color = colorLightOff;
         material.SetColor("_EmissionColor", colorOff * 5);

         if (mechanism.Length > 0)
            for (int i = 0; i < mechanism.Length; i++)
            {
               mechanism[i].tag = "electricityOff";
            }
      }
      else
      {
         anim.SetBool("Active", false);
         lightTrue.color = colorLightBlackout;
         material.SetColor("_EmissionColor", colorBlackout);
      }
   }
}
