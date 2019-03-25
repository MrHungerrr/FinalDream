using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHelperAI : MonoBehaviour
{
   public GameObject[] orbless = new GameObject[2];
   [HideInInspector]
   public OrblessAI[] orblessAI = new OrblessAI[2];
   [HideInInspector]
   public int orblessCount;


   public GameObject[] spider = new GameObject[2];
   [HideInInspector]
   public SpidersAI[] spiderAI = new SpidersAI[2];
   [HideInInspector]
   public int spiderCount;


   [HideInInspector]
   public Transform player;
   [HideInInspector]
   public bool night = true;
   [HideInInspector]
   public float blackoutDist = 75;
   [HideInInspector]
   public float overloadDist = 3;
   private float distBuf;


   // Start is called before the first frame update
   void Start()
   {
      night = true;
      orblessCount = orbless.Length;
      for (int i = 0; i < spiderCount; i++)
      {
         spiderAI[i] = spider[i].GetComponent<SpidersAI>();
      }

      for (int i = 0; i < orblessCount; i++)
      {
         orblessAI[i] = orbless[i].GetComponent<OrblessAI>();
      }
   }


   // Update is called once per frame
   void Update()
   {


   }

   public void Sound(Vector3 pos, float rad)
   {
      for(int i = 0; i<spiderCount; i++)
      {
         if(spider[i].active == true)
         {
            distBuf = (pos - spider[i].transform.position).magnitude;
            if (distBuf<=rad)
            {
               //spiderAI[i].Sound(pos);
            }
         }
      }

      for (int i = 0; i < orblessCount; i++)
      {
         if (orbless[i].active == true)
         {
            distBuf = (pos - orbless[i].transform.position).magnitude;
            if (distBuf <= rad)
            {
               orblessAI[i].Sound(pos);
            }
         }
      }
   }

}
