using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
   public GameObject[] mechanism;

   private EnemyHelperAI eHelpAI;

   public Renderer[] emissionLamps;
   private Material material;
   private int lampsQuant;
   private bool[] code = new bool[6];
   public bool power = false;

   private Color colorOn = new Color(0, 1, 0);
   private Color colorOff = new Color(0, 0, 0);

   private FMOD.Studio.EventInstance soundEngine;

   void Awake()
   {
      soundEngine = FMODUnity.RuntimeManager.CreateInstance("event:/Engine");
      lampsQuant = emissionLamps.Length;
      material = emissionLamps[0].material;
      soundEngine.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));

      for (int i = 0; i < lampsQuant; i++)
      {
         emissionLamps[i].material = material;
      }

      this.tag = "generator";

      PowerOff();
      
      for(int i=0; i<6; i++)
      {
         code[i] = false;
      }
   }


   public void TurnOn(byte nomber)
   {
      code[nomber] = true;

      if(code[0] && !code[1] && code[2] && code[3] && !code[4] && !code[5])
      {
         PowerOn();
      }
   }

   public void TurnOff(byte nomber)
   {
      code[nomber] = false;
      if (code[0] && !code[1] && code[2] && code[3] && !code[4] && !code[5])
      {
         PowerOn();
      }
   }



   private void PowerOn()
   {
      material.SetColor("_EmissionColor", colorOn * 2.5f);
      soundEngine.start();
      power = true;
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



   private void PowerOff()
   {
         soundEngine.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
         material.SetColor("_EmissionColor", colorOff);

         if (mechanism.Length > 0)
            for (int i = 0; i < mechanism.Length; i++)
            {
               mechanism[i].tag = "electricityOff";
            }
   }
}
