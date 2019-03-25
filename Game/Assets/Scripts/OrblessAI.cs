using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrblessAI : MonoBehaviour
{
   public GameObject player;
   private Vector3 dist3;
   private float dist;

    void Start()
    {
        
    }

    void Update()
    {
      dist3 = transform.position - player.transform.position;
      dist = dist3.magnitude;
    }


   public void Sound(Vector3 pos)
   {

   }


   private void OnTriggerStay(Collider harm)
   {



   }
}
