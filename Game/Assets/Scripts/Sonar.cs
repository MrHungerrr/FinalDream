using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{
   public AudioSource sonar;
   private float time;

    void Start()
    {
        
    }

   // Update is called once per frame
   void Update()
   {
      if (time > 0)
         time -= Time.deltaTime;
      else
      {
         sonar.Play();
         time = 1;
      }
   }
}
