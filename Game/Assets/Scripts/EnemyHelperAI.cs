using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHelperAI : MonoBehaviour
{
   [Header("Orblesses")]
   public Transform[] orbless;
   [HideInInspector]
   public OrblessAI[] orblessAI;
   [HideInInspector]
   public int orblessCount;
   [HideInInspector]
   public bool[] orblessPointBusy;

   [Header("Spiders")]
   public Transform[] spider;
   [HideInInspector]
   public SpidersAI[] spiderAI;
   [HideInInspector]
   public int spiderCount;
   [HideInInspector]
   public bool[] spiderPointBusy;


   [Header("Points")]
   public Transform[] point;
   [HideInInspector]
   public int pointCount;
   [HideInInspector]
   public Transform player;
   [HideInInspector]
   public bool night = true;
   [HideInInspector]
   public float blackoutDist = 75;
   [HideInInspector]
   public float overloadDist = 3;
   private float distBuf;


   private void Awake()
   {
      orblessPointBusy = new bool[point.Length];
      spiderPointBusy = new bool[point.Length];
      pointCount = point.Length;

      for (int i = 0; i < pointCount; i++)
      {
         orblessPointBusy[i] = false;
         spiderPointBusy[i] = false;
      }
   }

   // Start is called before the first frame update
   void Start()
   {
      night = true;
      orblessAI = new OrblessAI[orbless.Length];
      spiderAI = new SpidersAI[spider.Length];
      orblessCount = orbless.Length;
      spiderCount = spider.Length;


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
         if(spider[i].gameObject.active == true)
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
         if (orbless[i].gameObject.active == true)
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
