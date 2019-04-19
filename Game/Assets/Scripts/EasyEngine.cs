using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyEngine: MonoBehaviour
{
   public GameObject[] mechanism;

   public void PowerOn()
   {
      if (mechanism.Length > 0)
         for (int i = 0; i < mechanism.Length; i++)
         {
            mechanism[i].tag = "electricityOn";
         }
   }


}
