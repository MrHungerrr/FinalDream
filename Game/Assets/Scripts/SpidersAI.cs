﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpidersAI : MonoBehaviour
{
   private Animator spiderAnim;
   private const int damage = 1;
   private float health = 30;
   public bool battle;
   private bool far;


   //Атака
   private const int attackDist = 4;
   private const float attackSpeed = 0.45f;
   private const float attackPrepTime_N = 0.7f;
   private const float attackDur_N = 0.3f;
   private const float attackCD_N = 1.5f;
   private float attackPrepTime;
   private float attackDur;
   private float attackCD;
   private Vector3 attackDirect;
   private bool attackPrep;
   private bool attackRest;
   private bool attack;
   private float visible = 10f;
   private const float canglevisible = 70f;
   private const float chear = 3f;
   private float anglevisible = 70f;
   private float looseanglevisible = 140f;
   private float hear = 3f;
   private float loosehear = 5f;
   private bool lp = true;




    //Заморозка
    private const float freezingTime_N = 2.50f;
   private float freezingTime;
   private float freezing = 1;


   //Передвижение
   private Quaternion targetRotation;
   private const float speed = 0.2f;
   private Transform player;
   private BoxCollider col;
   private Vector3 direct;
   private Rigidbody rb;
   private NavMeshAgent agent;





   void Start()
   {
      player = GameObject.Find("Suit").transform;
      spiderAnim = GetComponent<Animator>();
      col = GetComponent<BoxCollider>();
      rb = GetComponent<Rigidbody>();
      agent = GetComponent<NavMeshAgent>();

      freezingTime = freezingTime_N;
      attackPrepTime = attackPrepTime_N;
      attackDur = attackDur_N;
      attackCD = attackCD_N;


      far = true;
      attack = false;
      attackRest = false;
      attackPrep = false;

        StartCoroutine("baseOffset");
   }

    IEnumerator baseOffset()
    {
        yield return new WaitForEndOfFrame();
        agent.baseOffset = -transform.position.y;
    }

   private void Update()
   {
      if (battle)
         BattleCalculate();
        Debug.Log(hear + " " + anglevisible);
   }

   private void FixedUpdate()
   {
      if (battle)
         Battle();
      PoiskPidora();

   }


    void PoiskPidora()
    {   
        
        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);

            if (dist < 1.5f)
            {
                BattleCalculate();
            }
            else if (dist < hear)
            {
                agent.SetDestination(player.transform.position);
                Debug.DrawRay(transform.position, player.position - transform.position, Color.red, visible);
                battle = true;
            }
            else if (dist < visible)
            {
                Quaternion look = Quaternion.LookRotation(player.position - transform.position);
                float angle = Quaternion.Angle(transform.rotation, look);

                if (angle < anglevisible)
                {
                    Ray ray = new Ray(transform.position , player.position - transform.position);
                    RaycastHit hit;
                    
                    if (Physics.Raycast(ray, out hit, visible))
                    {
                        //Debug.Log(hit.transform.tag);
                        Debug.DrawRay(transform.position , player.position - transform.position, Color.red, visible);
                        if (hit.transform.tag == "Player")
                        {
                            agent.SetDestination(player.transform.position);
                            battle = true;
                            StopCoroutine("LoosePlayer");
                            lp = true;
                        }
                        else if(hit.transform.name!= "Spider")
                        {
                            hear = loosehear;
                            anglevisible = looseanglevisible;
                            if (lp)
                                StartCoroutine("LoosePlayer");
                        }
                    }
                }
            }
        }
    }

    IEnumerator LoosePlayer()
    {
        lp = false;
        yield return new WaitForSeconds(5f);
        hear = chear;
        anglevisible = canglevisible;
        Debug.Log("Упустил");
    }



    //Атака(Вычисления)
    void BattleCalculate()
    {

      //Задержка перед атакой
      if (attackPrep)
      {
         Prepare();
      }


      //КД и обнуление переменных
      if (attackRest)
      {
         Zeroing();
      }


      //Восстановление от заморозки
      Freezing();
    }

   private void Prepare()
   {
      if (attackPrepTime > 0)
      {
         attackPrepTime -= Time.deltaTime * freezing;
         attackDirect = direct.normalized;
      }
      else
      {
         //spiderAnim.SetBool("Attack", true);
         attackDirect = direct.normalized;
         attackPrep = false;
         attack = true;

      }
   }

   private void Zeroing()
   {
      if (attackCD > 0)
      {
         attackCD -= Time.deltaTime * freezing;
      }
      else
      {
         attackPrepTime = attackPrepTime_N;
         attackDur = attackDur_N;
         attackCD = attackCD_N;
         attackPrep = false;
         attackRest = false;
         attack = false;
         far = true;
      }
   }

   private void Freezing()
   {
      if (freezingTime < freezingTime_N)
      {
         freezingTime += Time.deltaTime;
      }
      else
      {
         freezingTime = freezingTime_N;
      }

      freezing = freezingTime / freezingTime_N;
   }



   //Атака(Физика)
   void Battle()
   {
      //Поворот к игроку
      if (!attack && !attackRest)
      {
         //Rotate();
      }

      //Приближение к игроку
      if (far)
      {
        // Move();
      }

      //Атака
      if (attack)
      {
         Attack();
      }
   }

   /*private void Rotate()
   {
      direct = player.position - transform.position;
      targetRotation = Quaternion.LookRotation(direct);
      targetRotation.z = 0;
      targetRotation.x = 0;
      targetRotation = Quaternion.Slerp(transform.rotation, targetRotation, 14f * Time.deltaTime);
      rb.MoveRotation(targetRotation);
   }

   /*private void Move()
   {
      if ((direct.magnitude > attackDist))
      {
         rb.MovePosition(transform.position + direct.normalized * speed * freezing);
         //spiderAnim.SetBool("Walk", true);
      }
      else
      {
         //spiderAnim.SetBool("Walk", false);
         attackPrep = true;
         far = false;
      }
   }
   */
   private void Attack()
   {
      if (attackDur > 0)
      {
         rb.MovePosition(transform.position + attackDirect * attackSpeed * freezing);
         attackDur -= Time.fixedDeltaTime;
      }
      else
      {
         //spiderAnim.SetBool("Attack", false);
         attackRest = true;
         attack = false;
      }
   }




   //Урон паукам
   void OnTriggerEnter(Collider harm)
   {
      //Меч
      if (harm.tag == "sword")
      {
         TakeDamage(10.0f);
      }    
    }

   void OnTriggerStay(Collider harm)
   {
         //Огонь
         if (harm.tag == "fire")
         {
            TakeDamage(20 * Time.deltaTime);
         }

         //Лед
         if (harm.tag == "ice")
         {
            if (freezingTime > 0)
            {

               freezingTime -= Time.deltaTime * 4;
            }
            else
            {
               TakeDamage(health);
            }
         }
    }

   public void TakeDamage(float totalDamage)
   {
      health -= totalDamage;

      if (health <= 0)
      {
         col.enabled = false;
         GetComponent<SpidersAI>().enabled = false;
         agent.enabled = false;
      }
   }



   //Урон игроку
   void OnCollisionEnter(Collision target)
   {
      if (target.gameObject.tag == "Player" && attack)
         target.gameObject.GetComponent<PlayerScript>().TakeDamage(damage);
   }
}
