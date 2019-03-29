 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OrblessAI : MonoBehaviour
{
   [HideInInspector]
   public EnemyHelperAI eHelpAI;
   private NavMeshAgent agent;
   [HideInInspector]
   public GameObject player;
   private PlayerScript pScript;
   [HideInInspector]
   public Sonar sonScript;
   private Vector3 goal;
   private Vector3[] goals;
   private const int goalsCount = 3;
   private int pTarget_NomBuf;
   private int pTarget_Nom;
   private int[] pTargets_Nom;
   private bool pHave = true;
   private int p_i=0;
   private byte orb_Nom;

   //Реакция на силы игрока
   private float forceDangerDist;
   private float forceTime;
   private const float fireTime_N = 3f;
   private const float iceTime_N = -5f;
   private float stunTime;
   private const float stunTime_N = 10f;
   private Quaternion pLook;
   private float pAngle;

   private bool patrol = true;
   private bool patrolFar = true;
   private bool patrolMid = false;
   private bool patrolNear = false;
   private bool check = false;
   private bool checkLookAround = false;
   private bool chase = false;

   private GameObject chaseTarget;
   private const float chaseDistance = 12f;
   private const float patrolCD_N = 2f;
   private float patrolCD = 0f;
   private const float checkCD_N = 1.0f;
   private float checkCD = 0f;
   private Vector3 checkPos;


   void Start()
   {
      orb_Nom = eHelpAI.orbless_Nom;
      pScript = player.GetComponent<PlayerScript>();
      agent = GetComponent<NavMeshAgent>();
      goals = new Vector3[goalsCount];
      pTargets_Nom = new int[goalsCount];

      for (int i = 0; i < goalsCount; i++)
      {
         pTargets_Nom[i] = i;
         eHelpAI.pointsBusy[i, orb_Nom] = true;
         goals[i] = eHelpAI.points[i].transform.position;
      }

      pTarget_Nom = 0;
      goal = eHelpAI.points[0].transform.position;
      agent.SetDestination(goal);
   }



   void Update()
   {
      if (patrol)
      {
         Patrol();
      }

      if (check)
      {
         Check();
      }

      if (chase)
      {
         Chase();
      }
   }

   private void FixedUpdate()
   {
      Harm();
   }

   private void Patrol()
   {
      if (pHave)
      {
         if (Vector3.Distance(new Vector3(goal.x, 0, goal.z), new Vector3(transform.position.x, 0, transform.position.z)) <= 0.1f)
         {
            if (patrolCD <= 0)
            {
               if (p_i < goalsCount)
               {
                  pTarget_Nom = pTargets_Nom[p_i];
                  goal = eHelpAI.points[pTarget_Nom].transform.position;
                  agent.SetDestination(goal);
                  //Debug.Log("Безокий начал идти к " + goal);
                  patrolCD = checkCD_N;
                  p_i++;
               }
               else
               {
                  pHave = false;
               }
            }
            else
            {
               patrolCD -= Time.deltaTime;
            }
         }
         else
         {
            //Debug.Log("Безокий просто идет");
            //Всякие звуки пока безокий передвигается
         }
      }
      else
      {
         if (patrolFar)
         {
            FindPoint(goalsCount, player.transform.position, 50, 20);
            pHave = true;
            patrolFar = false;
            patrolMid = true;
            p_i = 0;
            Debug.Log("Безокий ищет вдалеке");
         }
         else if (patrolMid)
         {
            FindPoint(goalsCount, player.transform.position, 25, 10);
            pHave = true;
            patrolMid = false;
            patrolNear = true;
            p_i = 0;
            Debug.Log("Безокий ищет рядом");
         }
         else if (patrolNear)
         {
            FindPoint(goalsCount, player.transform.position, 15);
            pHave = true;
            patrolNear = false;
            patrolFar = true;
            p_i = 0;
            Debug.Log("Безокий ищет очень близко");
         }
      }
   }



   private void Check()
   {
      if (pHave)
      {
         if (Vector3.Distance(new Vector3(goal.x, 0, goal.z), new Vector3(transform.position.x, 0, transform.position.z)) <= 0.1f)
         {
            if (checkCD <= 0)
            {
               if (p_i < goalsCount)
               {
                  pTarget_Nom = pTargets_Nom[p_i];
                  goal = eHelpAI.points[pTarget_Nom].transform.position;
                  Debug.Log("Следующая точка для проверки места" + goal);
                  agent.SetDestination(goal);
                  checkCD = checkCD_N;
                  checkLookAround = false;
                  p_i++;
               }
               else
               {
                  pHave = false;
               }
               //Еще один звук?
            }
            else
            {
               checkCD -= Time.deltaTime;
            }
         }
         else
         {
            //Всякие звуки пока безокий передвигается
         }
      }
      else
      {
         Debug.Log("Безокий перестает искать добычу в месте");
         check = false;
         patrolCD = 4f;
         agent.speed = 6;
         patrol = true;
         patrolFar = false;
         patrolMid = true;
         patrolNear = false;
         pHave = false;
      }
   }



   private void Chase()
   {
      if (chaseTarget == player)
      {
         if (pScript.forceScare && (pScript.forceType == 1))
         {
            pLook = Quaternion.LookRotation(player.transform.position - transform.position);
            pAngle = Quaternion.Angle(transform.rotation, pLook);
            if (pAngle < 75)
            {
               Debug.Log("Безокий боится огня");
               goal = player.transform.position + (transform.position - player.transform.position).normalized * 5;
            }
            else
            {
               goal = player.transform.position;
            }
         }
         else
         {
            goal = player.transform.position;
         }
         if (Vector3.Distance(transform.position, player.transform.position) <= 0.2f)
         {
            Debug.Log("Безокий убил нас");
            //Смерть
         }
      }
      else
      {
         goal = player.transform.position;
         if (Vector3.Distance(transform.position, chaseTarget.transform.position) <= 0.5f)
         {
            Debug.Log("Безокий убил " + chaseTarget);
            //Смерть
         }
      }
      agent.SetDestination(goal);
   }



   //Найти одну точку вокруг таргета
   private void FindPoint(Vector3 target, float rad)
   {
      pTarget_NomBuf = pTarget_Nom;
      pTarget_Nom = eHelpAI.PointNear(target, rad, orb_Nom);

      if (pTarget_Nom >= 0)
      {
         goal = eHelpAI.points[pTarget_Nom].transform.position;
         eHelpAI.pointsBusy[pTarget_NomBuf, orb_Nom] = false;
         Debug.Log("Безокий нашел точку " + goal);
      }
      else
      {
         pTarget_Nom = pTarget_NomBuf;
         Debug.Log("Безокий НЕ НАШЕЛ ТОЧКУ");
         FindPoint(target, rad + 1);
      }


   }

   //Найти несколько точек вокруг таргета
   private void FindPoint(int count, Vector3 target, float rad)
   {
      for (int i = 0; i < count; i++)
      {
         pTarget_NomBuf = pTargets_Nom[i];
         pTargets_Nom[i] = eHelpAI.PointNear(target, rad, orb_Nom);

         if (pTargets_Nom[i] >= 0)
         {
            goals[i] = eHelpAI.points[pTargets_Nom[i]].transform.position;
            eHelpAI.pointsBusy[pTarget_NomBuf, orb_Nom] = false;
            Debug.Log("Безокий нашел точку " + goals[i]);
         }
         else
         {
            pTargets_Nom[i] = pTarget_NomBuf;
            Debug.Log("Безокий НЕ НАШЕЛ ТОЧКУ");
            FindPoint(count, target, rad + 1);
         }   
      }
   }

   //Найти одну точку в кольце вокруг таргета
   private void FindPoint(Vector3 target, float rad_B, float rad_S)
   {
      pTarget_NomBuf = pTarget_Nom;
      pTarget_Nom = eHelpAI.PointBetween(target, rad_B, rad_S, orb_Nom);

      if (pTarget_Nom >= 0)
      {
         goal = eHelpAI.points[pTarget_Nom].transform.position;
         eHelpAI.pointsBusy[pTarget_NomBuf, orb_Nom] = false;
         Debug.Log("Безокий нашел точку " + goal);
      }
      else
      {
         pTarget_Nom = pTarget_NomBuf;
         Debug.Log("Безокий НЕ НАШЕЛ ТОЧКУ");
         FindPoint(target, rad_B + 1, rad_S - 1);
      }
   }

   //Найти несколько точек в кольце вокруг таргета
   private void FindPoint(int count, Vector3 target, float rad_B, float rad_S)
   {
      for (int i = 0; i < count; i++)
      {
         pTarget_NomBuf = pTargets_Nom[i];
         pTargets_Nom[i] = eHelpAI.PointBetween(target, rad_B, rad_S, orb_Nom);

         if (pTargets_Nom[i] >= 0)
         {
            goals[i] = eHelpAI.points[pTargets_Nom[i]].transform.position;
            eHelpAI.pointsBusy[pTarget_NomBuf, orb_Nom] = false;
            Debug.Log("Безокий нашел точку " + goals[i]);
         }
         else
         {
            pTargets_Nom[i] = pTarget_NomBuf;
            Debug.Log("Безокий НЕ НАШЕЛ ТОЧКУ");
            FindPoint(count, target, rad_B + 1, rad_S - 1);
         } 
      }
   }



   public void Sound(Vector3 pos, GameObject source)
   {
      if (patrol)
      {

         Debug.Log("Безокий услышал что-то подозрительное");
         patrol = false;
         checkPos = pos;
         StartCoroutine(StartCheck());
      }

      if (check)
      {
         if (Vector3.Distance(pos, transform.position) < chaseDistance)
         {
            Debug.Log("Безокий 'нашел' добычу");
            check = false;
            chaseTarget = source;
            StartCoroutine(StartChase());
         }
         else
         {
            StartCoroutine(StartCheck());
         }
      }
   }



   IEnumerator StartCheck()
   {
      //Звуки всякие
      agent.SetDestination(transform.position);

      for (int i = 0; i < goalsCount; i++)
      {
         eHelpAI.pointsBusy[pTargets_Nom[i], orb_Nom] = false;
      }

      FindPoint(checkPos, 5);
      pTargets_Nom[0] = pTarget_Nom;
      goals[0] = goal;

      FindPoint(checkPos, 5, 10);
      pTargets_Nom[1] = pTarget_Nom;
      goals[1] = goal;

      FindPoint(checkPos, 5, 10);
      pTargets_Nom[2] = pTarget_Nom;
      goals[2] = goal;
      Debug.Log("Безокий нашел новые точки для проверки места");

      p_i = 1;
      pHave = true;
      agent.speed = 15;
      pTarget_Nom = pTargets_Nom[0];
      goal = eHelpAI.points[pTarget_Nom].transform.position;
      Debug.Log("Следующая точка для проверки места" + goal);
      checkCD = checkCD_N;
      checkLookAround = false;

      yield return new WaitForSeconds(1.0f);
      agent.SetDestination(goal);
      check = true;
   }

   IEnumerator StartChase()
   {
      //Звуки всякие
      agent.SetDestination(transform.position);
      yield return new WaitForSeconds(1.0f);
      agent.speed = 15;
      chase = true;
   }


   private void Harm()
   {
      //Загорание
      if (forceTime > 0)
      {
         forceTime -= Time.fixedDeltaTime;

         if(forceTime<fireTime_N)
         {
            //отближение + настроить две скорости
         }
         else
         {
            //уюегание
         }
      }

      //Заледенение
      if (forceTime < 0)
      {
         forceTime += Time.fixedDeltaTime;

         if (forceTime > fireTime_N)
         {

         }
         else
         {
            //заморозка
         }
      }
   }


   private void OnTriggerStay(Collider harm)
   {
      if(harm.tag == "fire")
      {
         forceTime += 2 * Time.fixedDeltaTime;
      }

      if (harm.tag == "ice")
      {
         forceTime -= 2 * Time.fixedDeltaTime;
      }

   }

}
