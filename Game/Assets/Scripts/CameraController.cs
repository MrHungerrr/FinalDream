using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

   public Transform player;
   public PlayerScript playerScript;
   public float smoothness;
   private Vector3 destonation;
   private Vector3 position;
   private Vector2 shake;
   public ParticleSystem snow;


   void Start ()
   {
      position = transform.position;
      snow.enableEmission = true;

   }
	
	void FixedUpdate ()
    {
        if (player)
        {

         destonation = new Vector3(position.x + player.position.x, position.y, position.z + player.position.z);
         transform.position = destonation;

        } 
	}
}
