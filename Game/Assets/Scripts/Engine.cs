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

   private FMOD.Studio.EventInstance soundEngine;

   void Awake()
   {
      soundEngine = FMODUnity.RuntimeManager.CreateInstance("event:/Engine");
      eHelpAI = GameObject.Find("EnemyHelper").GetComponent<EnemyHelperAI>();
      lampsQuant = emissionLamps.Length;
      blackoutDist = eHelpAI.blackoutDist;
      overloadDist = eHelpAI.overloadDist;
      material = emissionLamps[0].material;
      soundEngine.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));

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
      else if (blackout)
      {
         blackout = false;
         power = false;
         PowerOff();
      }
      else
         Power();
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
      material.SetColor("_EmissionColor", colorOn * 2.5f);
      soundEngine.start();

      if (mechanism.Length > 0)
         for (int i = 0; i < mechanism.Length; i++)
         {
            if (mechanism[i].tag == "door")
               mechanism[i].GetComponent<Door>().power = true;
            if (mechanism[i].tag == "lamp")
               mechanism[i].GetComponent<Lamp>().power = true;
            if (mechanism[i].tag == "computer")
               mechanism[i].GetComponent<Computer>().power = true;
            if (mechanism[i].tag == "terminal")
               mechanism[i].GetComponent<Terminal>().power = true;
            if (mechanism[i].tag == "generator")
               mechanism[i].GetComponent<Generator>().power = true;
         }
   }



   private void PowerOn(float intesivity)
   {
      
      anim.SetBool("Active", true);
      soundEngine.start();
      material.SetColor("_EmissionColor", colorOn * 4 * intesivity);
      lightTrue.color = colorLightOn;
      lightTrue.intensity = Mathf.Lerp(lightTrue.intensity, intesivity, 0.3f);
   }



   private void PowerOff()
   {
      if (!blackout)
      {
         powerNow = false;
         anim.SetBool("Active", false);
         soundEngine.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
         lightTrue.color = colorLightOff;
         material.SetColor("_EmissionColor", colorOff * 3.5f);

         if (mechanism.Length > 0)
            for (int i = 0; i < mechanism.Length; i++)
            {
               mechanism[i].tag = "electricityOff";
            }
      }
      else
      {
         soundEngine.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
         anim.SetBool("Active", false);
         lightTrue.color = colorLightBlackout;
         material.SetColor("_EmissionColor", colorBlackout);
      }
   }
}
