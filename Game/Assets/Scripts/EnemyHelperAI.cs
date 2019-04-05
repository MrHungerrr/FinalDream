using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHelperAI : MonoBehaviour
{
   [HideInInspector]
   public GameObject[] orblesses;
   [HideInInspector]
   public OrblessAI[] orblessAI;
   [HideInInspector]
   public int orblessCount;


   [HideInInspector]
   public GameObject[] spiders;
   [HideInInspector]
   public SpidersAI[] spiderAI;
   [HideInInspector]
   public int spiderCount;

   [HideInInspector]
   public Vector3[] points;
   [HideInInspector]
   public int pointCount;
   [HideInInspector]
   public bool[,] pointsBusy;

   [HideInInspector]
   public Transform player;
   public bool night = false;
   [HideInInspector]
   public float blackoutDist = 75;
   [HideInInspector]
   public float overloadDist = 10;
   private float distBuf;
   private bool rand;

   [HideInInspector]
   public byte orbless_Nom = 0;
   [HideInInspector]
   public byte spider_Nom = 1;

   [ContextMenu("AutoFill")]
   public void Fill()
   {


   }



   private void Awake()
   {
      orblesses = GameObject.FindGameObjectsWithTag("orbless");
      spiders = GameObject.FindGameObjectsWithTag("spider");
      GameObject[] pointsBuf = GameObject.FindGameObjectsWithTag("point");

      pointCount = pointsBuf.Length;
      points = new Vector3[pointCount];
      pointsBusy = new bool[pointCount, 2];


      for (int i = 0; i < pointCount; i++)
      {
         points[i] = pointsBuf[i].transform.position;
         Destroy(pointsBuf[i]);
         pointsBusy[i, orbless_Nom] = false;
         pointsBusy[i, spider_Nom] = false;
      }
   }

   // Start is called before the first frame update
   void Start()
   {
      orblessAI = new OrblessAI[orblesses.Length];
      spiderAI = new SpidersAI[spiders.Length];
      orblessCount = orblesses.Length;
      spiderCount = spiders.Length;
      
      for (int i = pointCount - 1; i >=1;i--)
      {
         int j = Random.Range(0, i-1);
         var temp = points[j];
         points[j] = points[i];
         points[i] = temp;
      }

      for (int i = 0; i < spiderCount; i++)
      {
         spiderAI[i] = spiders[i].GetComponent<SpidersAI>();
      }

      for (int i = 0; i < orblessCount; i++)
      {
         orblessAI[i] = orblesses[i].GetComponent<OrblessAI>();
      }

      Night();
   }



   public int PointNear(Vector3 pos, float rad, int EnemyType)
   {
      rand = !rand;
      if (rand)
      {
         for (int i = 0; i < pointCount; i++)
         {
            if (!pointsBusy[i, EnemyType])
            {
               //Debug.Log(Vector3.Distance(pos, point[i].transform.position));
               if (Vector3.Distance(pos, points[i]) <= rad)
               {
                  pointsBusy[i, EnemyType] = true;
                  return i;
               }
            }
         }
         return -1;
      }
      else
      {
         for (int i = pointCount-1; i >=0; i--)
         {
            if (!pointsBusy[i, EnemyType])
            {
               //Debug.Log(Vector3.Distance(pos, point[i].transform.position));
               if (Vector3.Distance(pos, points[i]) <= rad)
               {
                  pointsBusy[i, EnemyType] = true;
                  return i;
               }
            }
         }
         return -1;
      }
   }



   public int PointBetween(Vector3 pos, float rad_B, float rad_S, int EnemyType)
   {
      rand = !rand;
      if (rand)
      {
         for (int i = 0; i < pointCount; i++)
         {
            if (!pointsBusy[i, EnemyType])
            {
               if ((Vector3.Distance(pos, points[i]) <= rad_B) && (Vector3.Distance(pos, points[i]) >= rad_S))
               {
                  pointsBusy[i, EnemyType] = true;
                  return i;
               }
            }
         }
         return -1;
      }
      else
      {
         for (int i = pointCount - 1; i >= 0; i--)
         {
            if (!pointsBusy[i, EnemyType])
            {
               if ((Vector3.Distance(pos, points[i]) <= rad_B) && (Vector3.Distance(pos, points[i]) >= rad_S))
               {
                  pointsBusy[i, EnemyType] = true;
                  return i;
               }
            }
         }
         return -1;
      }
   }


   // Update is called once per frame
   void Update()
   {


   }


   public void Night()
   {
      if (night)
      {
         for (int i = 0; i < orblessCount; i++)
         {
            orblesses[i].SetActive(true);
         }
      }
      else
      {
         for (int i = 0; i < orblessCount; i++)
         {
            orblesses[i].SetActive(false);
         }
      }
   }

   public void Sound(Vector3 pos, float rad, GameObject source)
   {
      //Безокие не издают звуков(Хотя мейби потом сделать бегство животных от звука безоких)
      for (int i = 0; i < orblessCount; i++)
      {
         if (orblesses[i].active == true)
         {
            distBuf = (pos - orblesses[i].transform.position).magnitude;
            if (distBuf <= rad * 2)
            {
               orblessAI[i].Sound(pos, source);
            }
         }
      }


      //Чтобы паук не шел на звук паука
      if (source.tag != "spider")
         for (int i = 0; i < spiderCount; i++)
         {
            if (spiders[i].active == true)
            {
               distBuf = (pos - spiders[i].transform.position).magnitude;
               if (distBuf <= rad)
               {
                  //spiderAI[i].Sound(pos);
               }
            }
         }
   }

}
