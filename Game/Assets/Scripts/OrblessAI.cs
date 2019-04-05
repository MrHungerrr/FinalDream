 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OrblessAI : MonoBehaviour
{
   private EnemyHelperAI eHelpAI;
   private NavMeshAgent agent;
   private GameObject player;
   private PlayerScript pScript;
   private Vector3 mainTarget;
   private Vector3 goal;
   private Vector3[] goals;
   private const int goalsCount = 3;
   private int pTarget_NomBuf;
   private int pTarget_Nom;
   private int[] pTargets_Nom;
   private bool pHave = false;
   private int p_i = 0;
   private byte orb_Nom;
   [HideInInspector]
   public float orbGlitchInt = 0.1f;
   private GlitchEffect glitch;

   //Реакция на силы игрока
   private float nervous;
   private float forceDangerDist;
   private float forceTime;
   private const float fireTime_N = 2f;
   private const float iceTime_N = -5f;
   private const float stunTime_N = 10f;
   private Quaternion pLook;
   private float pAngle;
   private bool hurt;

   private bool patrol = true;
   private bool patrolFar = true;
   private bool patrolMid = false;
   private bool patrolNear = false;
   private bool check = false;
   private bool checkLookAround = false;
   private bool chase = false;

   private GameObject chaseTarget;
   private const float chaseDistance = 12f;
   private Vector3 chaseFearVec;
   private const float patrolCD_N = 2f;
   private float patrolCD = 0f;
   private const float checkCD_N = 1.0f;
   private float checkCD = 0f;
   private Vector3 checkPos;

   private const float fastSpeed = 15f;
   private const float midSpeed = 10f;
   private const float slowSpeed = 6f;


   void Start()
   {
      eHelpAI = GameObject.Find("EnemyHelper").GetComponent<EnemyHelperAI>();
      player = GameObject.FindGameObjectWithTag("Player");
      glitch = GameObject.Find("Main Camera").GetComponent<GlitchEffect>();
      orb_Nom = eHelpAI.orbless_Nom;
      pScript = player.GetComponent<PlayerScript>();
      agent = GetComponent<NavMeshAgent>();
      goals = new Vector3[goalsCount];
      pTargets_Nom = new int[goalsCount];

      for (int i = 0; i < goalsCount; i++)
      {
         pTargets_Nom[i] = i;
         eHelpAI.pointsBusy[i, orb_Nom] = true;
         goals[i] = eHelpAI.points[i];
      }

      pTarget_Nom = 0;
      goal = eHelpAI.points[0];
      Debug.Log("Ura, Poehali k " + goal);
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

      Speed();
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
                  goal = eHelpAI.points[pTarget_Nom];
                  agent.SetDestination(goal);
                  Debug.Log("Безокий начал идти к " + goal);
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
         StartCoroutine(Glitch(0.3f));
         Target();
         if (patrolFar)
         {
            FindPoint(goalsCount, mainTarget, 50, 20);
            pHave = true;
            patrolFar = false;
            patrolMid = true;
            pTarget_Nom = pTargets_Nom[0];
            goal = eHelpAI.points[0];
            agent.SetDestination(goal);
            patrolCD = checkCD_N;
            p_i = 1;
            Debug.Log("Безокий ищет вдалеке");
         }
         else if (patrolMid)
         {
            FindPoint(goalsCount, mainTarget, 25, 10);
            pHave = true;
            patrolMid = false;
            patrolNear = true;
            pTarget_Nom = pTargets_Nom[0];
            goal = eHelpAI.points[0];
            agent.SetDestination(goal);
            patrolCD = checkCD_N;
            p_i = 1;
            Debug.Log("Безокий ищет рядом");
         }
         else if (patrolNear)
         {
            FindPoint(goalsCount, mainTarget, 15);
            pHave = true;
            patrolNear = false;
            patrolFar = true;
            pTarget_Nom = pTargets_Nom[0];
            goal = eHelpAI.points[0];
            agent.SetDestination(goal);
            patrolCD = checkCD_N;
            p_i = 1;
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
                  goal = eHelpAI.points[pTarget_Nom];
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
         pLook = Quaternion.LookRotation(player.transform.position - transform.position);
         pAngle = Quaternion.Angle(player.transform.rotation, pLook);
         if (pAngle < 75)
         {

            if (pScript.forceScare && (pScript.forceType == 1) && Vector3.Distance(transform.position, player.transform.position) > 2f)
            {
               agent.speed = slowSpeed;
               Debug.Log("Безокий боится огня");
               goal = player.transform.position + (transform.position - player.transform.position).normalized * 5 + chaseFearVec;
               nervous = 0;
            }
            else
            {
               nervous += Time.deltaTime;
               goal = player.transform.position;
            }
         }
         else
         {
            nervous += Time.deltaTime * 5;
            goal = player.transform.position;
         }

         if (Vector3.Distance(transform.position, player.transform.position) <= 0.2f)
         {
            pScript.TakeDamage(4);
            Debug.Log("Безокий убил нас");
            chase = false;
            patrolCD = 4f;
            nervous = 0;
            patrol = true;
            patrolFar = false;
            patrolMid = true;
            patrolNear = false;
            pHave = false;
            //Смерть
         }
      }
      else
      {
         goal = chaseTarget.transform.position;
         if (Vector3.Distance(transform.position, chaseTarget.transform.position) <= 0.5f)
         {
            Debug.Log("Безокий убил " + chaseTarget);
            chase = false;
            patrolCD = 4f;
            nervous = 0;
            patrol = true;
            patrolFar = false;
            patrolMid = true;
            patrolNear = false;
            pHave = false;
            //Смерть
         }
      }
      agent.SetDestination(goal);
   }



   private void Speed()
   {

      if (patrol)
      {
         if (nervous > 0)
         {
            agent.speed = midSpeed;
            nervous -= Time.deltaTime;
         }
         else
         {
            agent.speed = slowSpeed;
         }
      }

      if (check)
      {
         agent.speed = midSpeed;
      }

      if (chase)
      {
         if (hurt)
         {
            if (forceTime < 0)
            {
               agent.speed = fastSpeed * ((3 + forceTime) / 3);
            }
            else
            {
               agent.speed = fastSpeed;
            }
         }
         else
         {
            if (nervous > 3)
            {

               agent.speed = fastSpeed;
            }
            else
            {
               agent.speed = slowSpeed;
            }
         }
      }
   }


   private void Target()
   {
      if(player.activeSelf)
      {
         mainTarget = player.transform.position;
      }
      else
      {
         mainTarget = transform.position;
      }
   }


   //Найти одну точку вокруг таргета
   private void FindPoint(Vector3 target, float rad)
   {
      pTarget_NomBuf = pTarget_Nom;
      pTarget_Nom = eHelpAI.PointNear(target, rad, orb_Nom);

      if (pTarget_Nom >= 0)
      {
         goal = eHelpAI.points[pTarget_Nom];
         eHelpAI.pointsBusy[pTarget_NomBuf, orb_Nom] = false;
         Debug.Log("Безокий нашел точку " + goal);
      }
      else
      {
         pTarget_Nom = pTarget_NomBuf;
         Debug.Log("Безокий НЕ НАШЕЛ ТОЧКУ");
         FindPoint(target, rad + 5);
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
            goals[i] = eHelpAI.points[pTargets_Nom[i]];
            eHelpAI.pointsBusy[pTarget_NomBuf, orb_Nom] = false;
            Debug.Log("Безокий нашел точку " + goals[i]);
         }
         else
         {
            pTargets_Nom[i] = pTarget_NomBuf;
            Debug.Log("Безокий НЕ НАШЕЛ ТОЧКУ");
            FindPoint(target, rad + 5);
            pTargets_Nom[i] = pTarget_Nom;
            goals[i] = goal;
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
         goal = eHelpAI.points[pTarget_Nom];
         eHelpAI.pointsBusy[pTarget_NomBuf, orb_Nom] = false;
         Debug.Log("Безокий нашел точку " + goal);
      }
      else
      {
         pTarget_Nom = pTarget_NomBuf;
         Debug.Log("Безокий НЕ НАШЕЛ ТОЧКУ");
         FindPoint(target, rad_B + 5, rad_S - 5);
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
            goals[i] = eHelpAI.points[pTargets_Nom[i]];
            eHelpAI.pointsBusy[pTarget_NomBuf, orb_Nom] = false;
            Debug.Log("Безокий нашел точку " + goals[i]);
         }
         else
         {
            pTargets_Nom[i] = pTarget_NomBuf;
            Debug.Log("Безокий НЕ НАШЕЛ ТОЧКУ");
            FindPoint(target, rad_B + 5, rad_S - 5);
            pTargets_Nom[i] = pTarget_Nom;
            goals[i] = goal;
         }
      }
   }



   public void Sound(Vector3 pos, GameObject source)
   {
      if (patrol)
      {

         //Debug.Log("Безокий услышал что-то подозрительное");
         if (Vector3.Distance(pos, transform.position) < chaseDistance)
         {
            //Debug.Log("Безокий 'нашел' добычу");
            patrol = false;
            check = false;
            chaseTarget = source;
            StartCoroutine(StartChase());
         }
         else
         {
            patrol = false;
            checkPos = pos;
            StartCoroutine(StartCheck());
         }
      }
      if (check)
      {
         if (Vector3.Distance(pos, transform.position) < chaseDistance)
         {
            //Debug.Log("Безокий 'нашел' добычу");
            check = false;
            chaseTarget = source;
            StartCoroutine(StartChase());
         }
         else
         {
            StartCoroutine(StartCheck());
         }
      }

      if(hurt)
      {
         patrol = false;
         check = false;
         chase = true;
      }
   }



   IEnumerator StartCheck()
   {
      //Звуки всякие
      agent.SetDestination(transform.position);
      StartCoroutine(Glitch(0.6f));
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
      nervous = 60;
      pTarget_Nom = pTargets_Nom[0];
      goal = eHelpAI.points[pTarget_Nom];
      Debug.Log("Следующая точка для проверки места" + goal);
      checkCD = checkCD_N;
      checkLookAround = false;

      yield return new WaitForSeconds(1.0f);
      agent.SetDestination(goal);
      check = true;
   }

   public IEnumerator StartChase()
   {
      //Звуки всякие

      StartCoroutine(Glitch(2f));
      agent.SetDestination(transform.position);
      yield return new WaitForSeconds(2.0f);
      chase = true;
   }

   IEnumerator Stun()
   {
      patrol = false;
      check = false;
      chase = false;
      //Звуки всякие
      agent.SetDestination(transform.position);
      yield return new WaitForSeconds(stunTime_N);
      Debug.Log("Безокий перестает искать добычу в месте");
      patrolCD = 0f;
      chase = false;
      nervous = 120;
      patrol = true;
      patrolFar = false;
      patrolMid = true;
      patrolNear = false;
      pHave = false;
      agent.speed = 15;
      chase = true;
   }

   IEnumerator Glitch(float max)
   {
      orbGlitchInt = Random.Range(0.1f, max);
      glitch.flipIntensity = Random.Range(0.1f, max);
      glitch.colorIntensity = Random.Range(0.1f, max);
      glitch.intensity = Random.Range(0.1f, max);
      yield return new WaitForSeconds(max*2);
      orbGlitchInt = 0.1f;
      glitch.flipIntensity = 0;
      glitch.colorIntensity = 0;
      glitch.intensity = 0;
   }

   private void Harm()
   {
      //Загорание
      if (forceTime > 0)
      {
         forceTime -= Time.fixedDeltaTime;

         if(forceTime<fireTime_N)
         {
            chaseFearVec = (transform.position - player.transform.position).normalized * forceTime * 2;
         }
         else
         {
            hurt = false;
            Debug.Log("Убегает");
            //Звук
            hurt = false;
            chase = false;
            FindPoint(player.transform.position, 50, 40);
            goals[2] = goal;   
            patrolCD = 4f;
            nervous = 120;
            agent.speed = 10;
            patrol = true;
            patrolFar = false;
            patrolMid = true;
            patrolNear = false;
            p_i = 2;
            pHave = true;
         }
      }

      //Заледенение
      if (forceTime < 0)
      {
         forceTime += Time.fixedDeltaTime;

         if (forceTime > iceTime_N)
         {
            nervous += Time.fixedDeltaTime * 5;
         }
         else
         {
            hurt = false;
            Debug.Log("Заморозился");
            StartCoroutine(Stun());
         }
      }
   }


   private void OnTriggerStay(Collider harm)
   {
      if(harm.tag == "fire")
      {
         hurt = true;
         forceTime += 2 * Time.fixedDeltaTime;
      }

      if (harm.tag == "ice")
      {
         hurt = true;
         forceTime -= 2 * Time.fixedDeltaTime;
      }

   }

   private void OnTriggerExit(Collider harm)
   {
      if (harm.tag == "fire" || harm.tag == "ice")
      {
         hurt = false;
      }
   }



   public void Kill(GameObject target)
   {
      patrol = false;
      check = false;
      chaseTarget = target;
      StartCoroutine(StartChase());
   }
}
