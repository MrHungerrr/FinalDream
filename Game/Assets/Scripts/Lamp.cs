using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{

   [HideInInspector]
   public bool power = false;

   public Renderer lamp;
   private Material material;
   public Light pointLight;

   private Color colorOn = new Color(1, 1, 1);
   private Color colorOff = new Color(0, 0, 0);


   void Start()
   {
      material = lamp.GetComponent<Renderer>().material;

      if (this.tag == "actionWantOn")
      {
         power = true;
         PowerOn();
      }

      if (this.tag == "actionOff")
      {
         power = false;
         PowerOff();
      }
   }



   void Update()
   {

      if (this.tag == "actionWantOn" && !power)
      {
         power = true;
         PowerOn();
      }

      if (this.tag == "actionOff" && power)
      {
         power = false;
         PowerOff();
      }
   }



   private void PowerOn()
   {
         material.SetColor("_EmissionColor", colorOn * 3);
         pointLight.intensity = 1.5f;

   }



   private void PowerOff()
   {
      material.SetColor("_EmissionColor", colorOff);
      pointLight.intensity = 0f;
   }



}
