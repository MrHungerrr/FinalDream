using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{


   public bool power;
   public GameObject[] mechanism = new GameObject[0];


   public byte temp;
   private const float forceTime = 2;
   private float iceTime = forceTime;
   private float fireTime = forceTime;
   private bool act;


   void Start ()
   {
      if (power)
         this.tag = "actionOn";
      else
         this.tag = "actionOff";
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
      if (mechanism.Length > 0)
         for (int i = 0; i < mechanism.Length; i++)
         {
            mechanism[i].tag = "actionWantOn";
         }
   }

   private void PowerOff()
   {
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
         switch (temp)
         {
            case 2:

               break;
            case 1:

               break;
            case 0:

               break;
         }
      }
      else
      {
         iceTime = Mathf.Lerp(iceTime, forceTime, 0.1f);
         fireTime = Mathf.Lerp(fireTime, forceTime, 0.1f);
      }

   }



}
