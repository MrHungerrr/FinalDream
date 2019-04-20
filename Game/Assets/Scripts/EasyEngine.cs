using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyEngine: MonoBehaviour
{
   public GameObject[] mechanism;

   private void Start()
   {
      this.tag = "easyEngine";
   }
   public void PowerOn()
   {
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


}
