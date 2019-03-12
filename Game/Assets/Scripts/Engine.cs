using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{


   public bool power;
   public GameObject[] mechanism = new GameObject[0];

   [HideInInspector]
   public GameObject[] lights = new GameObject[4];
   private Material[] materials = new Material[4];
   private int lightsQuant;
   [HideInInspector]
   public Light pointLight_active;
   public byte temp;
   private const float forceTime = 2;
   private float iceTime = forceTime;
   private float fireTime = forceTime;
   private bool act;
   private Animator anim;

   private Color colorOn = new Color(0,1,0);
   private Color colorOff = new Color (1,0,0);
   private Color colorIce = new Color(0, 0.5f, 1.0f);
   private Color colorFire = new Color(1.0f, 0.5f, 0);

   private Color colorLightOn = new Color(0.4f, 1, 0.4f);
   private Color colorLightOff = new Color(1, 0.35f, 0);
   private Color colorLightIce = new Color(0.5f, 0.75f, 1.0f);
   private Color colorLightFire = new Color(1.0f, 0.7f, 0.3f);


   void Start()
   {
      lightsQuant = lights.Length;

      for (int i = 0; i < lightsQuant; i++)
      {
         materials[i] = lights[i].GetComponent<Renderer>().material;
      }

      anim = GetComponent<Animator>();
      if (power && temp == 1)
      {
         this.tag = "actionOn";
         PowerOn();
      }
      else
      {
         this.tag = "actionOff";
         PowerOff();
      }
   }
	


	void Update ()
   {
      if ((this.tag == "actionWantOn" || this.tag == "actionOn") && (temp == 1))
      {
         this.tag = "actionOn";
      }
      else
      {
         this.tag = "actionOff";
      }
      if (this.tag == "actionOn" && !power)
      {
         power = true;
         PowerOn();
      }

      if (this.tag == "actionOff" && power)
      {
         power = false;
         PowerOff();
      }


      if(!act && iceTime == forceTime && fireTime == forceTime)
      Temperature();

	}



   private void PowerOn()
   {
      anim.SetBool("Active", true);
      for (int i = 0; i < lightsQuant; i++)
      {
         materials[i].SetColor("_EmissionColor", colorOn * 5);
         pointLight_active.color = colorLightOn;
      }

      if (mechanism.Length > 0)
         for (int i = 0; i < mechanism.Length; i++)
         {
            mechanism[i].tag = "actionWantOn";
         }
   }



   private void PowerOff()
   {
      anim.SetBool("Active", false);
      switch (temp)
      {
         case 2:
            {
               for (int i = 0; i < lightsQuant; i++)
                  materials[i].SetColor("_EmissionColor", colorFire * 5);
               pointLight_active.color = colorLightFire;
               break;
            }
         case 1:
            {
               for (int i = 0; i < lightsQuant; i++)
                  materials[i].SetColor("_EmissionColor", colorOff * 5);
               pointLight_active.color = colorLightOff;
               break;
            }
         case 0:
            {
               for (int i = 0; i < lightsQuant; i++)
                  materials[i].SetColor("_EmissionColor", colorIce * 5);
               pointLight_active.color = colorLightIce;
               break;
            }
      }

      if (mechanism.Length > 0)
         for (int i = 0; i < mechanism.Length; i++)
         {
            mechanism[i].tag = "actionOff";
         }
   }



   private void OnTriggerStay(Collider force)
   {
      act = true;

      if (force.tag == "ice")
      {
         iceTime -= Time.deltaTime;
         fireTime += Time.deltaTime;
         if (temp > 0 && iceTime <= 0)
         {
            temp--;
            Temperature();
            iceTime = 3;
            fireTime = 1;
         }
      }

      if (force.tag == "fire")
      {
         fireTime -= Time.deltaTime;
         iceTime += Time.deltaTime;
         if (temp < 2 && fireTime <= 0)
         {
            temp++;
            Temperature();
            iceTime = 1;
            fireTime = 3;
         }
      }


   }



   private void OnTriggerExit(Collider force)
   {
      if (force.tag == "ice" || force.tag == "fire")
         act = false;
   }



   private void Temperature()
   {



      if (act)
      {
         PowerOff();
      }
      else
      {
         iceTime = Mathf.Lerp(iceTime, forceTime, 0.1f);
         fireTime = Mathf.Lerp(fireTime, forceTime, 0.1f);
      }

   }



}
