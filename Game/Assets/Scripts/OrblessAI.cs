 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OrblessAI : MonoBehaviour
{

   public EnemyHelperAI eHelpAI;
   private NavMeshAgent agent;
   public GameObject player;
   private PlayerScript pScript;
   public Sonar sonScript;
   private Vector3 goal;
   private Vector3[] goals = new Vector3[5];
   private int goalsCount;
   private int pTarget_NomBuf;
   private int pTarget_Nom = 1;
   private int[] pTargets_Nom;   
   private byte orb_Nom;

   private bool patrol = true;
   private bool check = false;
   private bool chase = false;
   private bool napor = true;

   private float nervous;
   private float nervCoef;
   private GameObject chaseTarget;
   private const float chaseDistance = 12f;
   private const float checkCD_N = 1.0f;
   private float checkCD = 1.0f;

   [ContextMenu("AutoFill")]
   public void Fiil()
   {
      eHelpAI = GameObject.FindGameObjectWithTag("enemyHelper").GetComponent<EnemyHelperAI>();
      player = GameObject.FindGameObjectWithTag("Player");
      sonScript = GameObject.Find("Sonar").GetComponent<Sonar>();
   }


   void Start()
   {
      orb_Nom = eHelpAI.orbless_Nom;
      pScript = player.GetComponent<PlayerScript>();
      agent = GetComponent<NavMeshAgent>();
      goal = new Vector3(2, 0, 2);
      agent.SetDestination(goal);
      goalsCount = goals.Length;
   }

    void Update()
    {

       if(patrol)
       {
         Nervous();
         Patrol();
       }

       if(check)
       {
          Check();
       }

       if(chase)
       {
          Chase();
       }
       
   }


   private void Nervous()
   {
      if(napor)
      {
         if(nervous > 5)
         {
            nervCoef = (-Mathf.Abs(sonScript.dist - 20) -10) / 100;
         }
         else
         {
            napor = false;
            //Debug.Log("Безокий начал отступать");
         }
      }
      else
      {
         if (nervous < 55)
         {
            nervCoef = (Mathf.Abs(sonScript.dist - 40) + 10) / 100;
         }
         else
         {
            napor = true;
           // Debug.Log("Безокий начал наступать");
         }
      }

      nervous += nervCoef*Time.deltaTime;
      //Debug.Log(nervous);

   }


   private void Patrol()
   {
      if (Vector3.Distance(new Vector3(goal.x, 0 ,goal.z), new Vector3(transform.position.x, 0, transform.position.z))<=0.1f)
      {
         pTarget_NomBuf = pTarget_Nom;
         FindPoint(player.transform.position, nervous, nervous-5);
         eHelpAI.pointsBusy[pTarget_NomBuf, orb_Nom] = false;
         //Debug.Log("Безокий Начал идти к " + goal);
      }
      else
      {
         // Debug.Log("Безокий просто идет");
         //Всякие звуки пока безокий передвигается
      }
   }

   private void Check()
   {
      if (Vector3.Distance(goal, transform.position) <= 1)
      {
         if (checkCD <= 0)
         {
            //Debug.Log("Безокий пытается найти добычу");
            pTarget_NomBuf = pTarget_Nom;
            FindPoint(player.transform.position, 15);
            eHelpAI.pointsBusy[pTarget_NomBuf, orb_Nom] = false;
            agent.SetDestination(goal);
            checkCD = checkCD_N;
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

      if (nervous <= 0)
      {
         //Debug.Log("Безокий перестает искать добычу");
         check = false;
         FindPoint(player.transform.position, 20, 5);
         StartCoroutine(StartPatrol());
      }
   }
   

   private void Chase()
   {
      if (chaseTarget == player)
      {
         if (pScript.forceScare && (pScript.forceType == 1))
         {
            //Debug.Log("Безокий боится огня");
            goal = player.transform.position + (transform.position - player.transform.position).normalized * 5;
         }
         else
         {
            goal = player.transform.position;
            if (Vector3.Distance(transform.position, player.transform.position) <= 0.5f)
            {
               Debug.Log("Безокий убил нас");
               //Смерть
            }
         }
         agent.SetDestination(goal);
      }
      else
      {
         goal = player.transform.position;
         agent.SetDestination(goal);
         if (Vector3.Distance(transform.position, chaseTarget.transform.position) <= 0.5f)
         {
            //Debug.Log("Безокий убил " + chaseTarget);
            //Смерть
         }

      }

   }



   private void FindPoint( Vector3 target, float rad)
   {
      pTarget_Nom = eHelpAI.PointNear(target, rad, orb_Nom);
      if (pTarget_Nom >= 0)
      {
         //Debug.Log(pTarget_Nom + "    " + orb_Nom);
         goal = eHelpAI.points[pTarget_Nom].transform.position;
         //Debug.Log("Безокий нашел точку " + goal);
         agent.SetDestination(goal);

      }
      else
      {
         //Debug.Log("Безокий НЕ НАШЕЛ ТОЧКУ");
         FindPoint(target, rad + 1);
      }
   }

   private void FindPoint(Vector3 target, float rad_B, float rad_S)
   {
      pTarget_Nom = eHelpAI.PointBetween(target, rad_B, rad_S, orb_Nom);
      if (pTarget_Nom >= 0)
      {
         //Debug.Log(pTarget_Nom + "    " + orb_Nom);
         goal = eHelpAI.points[pTarget_Nom].transform.position;
         //Debug.Log("Безокий нашел точку " + goal);
         agent.SetDestination(goal);
      }
      else
      {
         //Debug.Log("Безокий НЕ НАШЕЛ ТОЧКУ");
         FindPoint(target, rad_B + 1, rad_S - 1);
      }
   }



   public void Sound(Vector3 pos, GameObject source)
   {
      if (patrol)
      {

         //Debug.Log("Безокий услышал что-то подозрительное");
         patrol = false;
         FindPoint(pos, 4);
         StartCoroutine(StartCheck());
      }

       if(check)
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
            nervous += 30f;
         }
      }
       

   }

   IEnumerator StartPatrol()
   {
      //Звуки всякие
      yield return new WaitForSeconds(1.0f);
      agent.speed = 5;
      patrol = true;
   }

   IEnumerator StartCheck()
   {
      //Звуки всякие
      yield return new WaitForSeconds(0.5f);
      nervous = 60;
      agent.speed = 10;
      check = true;
   }

   IEnumerator StartChase()
   {
      //Звуки всякие
      yield return new WaitForSeconds(1.0f);
      chase = true;
   }

   private void OnTriggerStay(Collider harm)
   {



   }
   
}
