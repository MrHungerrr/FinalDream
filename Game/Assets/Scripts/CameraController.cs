using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

   private Vector3 position;

   private void Awake()
   {
      position = transform.position;
   }

   void Start ()
   {

      transform.position = position;

   }

   void FixedUpdate ()
   {

	}
}
