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
      player.GetComponent<Sonar>().Distance(dist);
      Debug.Log(dist);
    }
}
