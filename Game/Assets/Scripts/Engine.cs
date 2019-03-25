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
   public EnemyHelperAI enemyHelpAI;
   private float distBuf;
   private float dist;
   private float blackoutDist;
   private float overloadDist;


   [HideInInspector]
   public GameObject[] lights = new GameObject[4];
   private Material[] materials = new Material[4];
   private int lightsQuant;
   [HideInInspector]
   public Light pointLight_active;
   private bool act; 
   private Animator anim;

   private Color colorOn = new Color(0,1,0);
   private Color colorOff = new Color (1,0,0);
   private Color colorBlackout = new Color(0, 0, 0);

   private Color colorLightOn = new Color(0.4f, 1, 0.4f);
   private Color colorLightOff = new Color(1, 0.35f, 0);
   private Color colorLightBlackout = new Color(0, 0, 0);

   void Start()
   {
      lightsQuant = lights.Length;
      blackoutDist = enemyHelpAI.blackoutDist;
      overloadDist = enemyHelpAI.overloadDist;

      for (int i = 0; i < lightsQuant; i++)
      {
         materials[i] = lights[i].GetComponent<Renderer>().material;
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
	


	void Update ()
   {
      if (enemyHelpAI.night)
      {
         dist = blackoutDist+1;
         for (int i = 0; i < enemyHelpAI.orblessCount; i++)
         {
            distBuf = (transform.position - enemyHelpAI.orbless[i].position).magnitude;
            if (dist > distBuf)
               dist = distBuf;
         }


         if (dist <= overloadDist)
         {
            blackout = false;
            PowerOn((1 / ((dist - 0.5f) * (dist - 0.5f))) + Random.Range(-0.1f, 0.1f));
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
      pointLight_active.color = colorLightOn;

      for (int i = 0; i < lightsQuant; i++)
      {
         materials[i].SetColor("_EmissionColor", colorOn * 5);
      }

      if (mechanism.Length > 0)
         for (int i = 0; i < mechanism.Length; i++)
         {
            mechanism[i].tag = "electricityOn";
         }
   }

   private void PowerOn(float intesivity)
   {
      anim.SetBool("Active", true);

      for (int i = 0; i < lightsQuant; i++)
      {
         materials[i].SetColor("_EmissionColor", colorOn * 5 * intesivity);

      }

      pointLight_active.color = colorLightOn;
      pointLight_active.intensity = Mathf.Lerp(pointLight_active.intensity, intesivity, 0.3f);
   }



   private void PowerOff()
   {
      if (!blackout)
      {
         powerNow = false;
         anim.SetBool("Active", false);
         pointLight_active.color = colorLightOff;

         for (int i = 0; i < lightsQuant; i++)
         {
            materials[i].SetColor("_EmissionColor", colorOff * 5);
         }


         if (mechanism.Length > 0)
            for (int i = 0; i < mechanism.Length; i++)
            {
               mechanism[i].tag = "electricityOff";
            }
      }
      else
      {
         anim.SetBool("Active", false);
         pointLight_active.color = colorLightBlackout;

         for (int i = 0; i < lightsQuant; i++)
         {
            materials[i].SetColor("_EmissionColor", colorBlackout);
         }
      }
   }
}
