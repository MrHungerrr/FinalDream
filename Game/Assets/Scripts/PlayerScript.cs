using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PlayerScript : MonoBehaviour
{
   //Костюм
   [Header("Suit")]
   public GameObject legs;
   private Animator playerAnim;
   private Animator legsAnim;
   private Vector3 legsOffset;
   private Transform legsTrans;
   public Light[] lights_color = new Light[3];


   //События и действия
   private bool idle = false;
   private bool action = false;


   //Сила и мана
   [Header("Force")]
   public GameObject force;
   private ParticleSystem partSys;
   private const float manaMax = 40;
   public Collider forceCol;
   [HideInInspector]
   public bool forceAct;
   private float mana;
   private bool fire;
   public Material mana_material;
   private Color mana_color;
   private float mana_intensity;


   //Здоровье
   [HideInInspector]
   public int protectLevel;
   [HideInInspector]
   public int health;
   private const int protectLevelMax = 4;
   private const int healthMax = 4;
   private const float regenTime_N = 4.0f;
   private float regenTime;
  // [HideInInspector]
   public Material hp_material;
   private Color protectLeavel_color;
 //  private float hp_intensity;


   //Движение
   private const float speedWalk = 0.06f;
   private const float speedRun = 0.1f;
   private float hitDist = 0.0f;
   private float movementAngle;
   private Plane playerPlane;
   private Quaternion movementRotation;
   private Quaternion targetRotation;
   [HideInInspector]
   public Vector3 targetPoint;
   private Vector3 inputMove;
   private Vector3 movement;
   private Rigidbody rb;
   private Ray lookRay;
   [HideInInspector]
   public bool runAct;


   //Прыжок
   private Vector3 jumpVector = new Vector3(0,1400,0);
   [HideInInspector]
   public bool jumpAct = false;
   private bool jumpBounce = true;
   private bool jumpFall = true;
   private bool ground = true;
   [HideInInspector]
   public float jumpCD = 0;
   private const float jumpCD_N = 1.0f;
   private float jumpTime;
   private const float jumpTime_N = 0.85f;




   private void Awake()
   {
      partSys = force.GetComponent<ParticleSystem>();
      legsTrans = legs.GetComponent<Transform>();
      legsAnim = legs.GetComponent<Animator>();
      legsOffset = legsTrans.position;
      playerAnim = GetComponent<Animator>();
      rb = GetComponent<Rigidbody>();
   }

   private void Start()
   {
      partSys.enableEmission = false;
      protectLeavel_color = new Color(0, 1, 0, 1);
      hp_material.SetColor("_EmissionColor",protectLeavel_color);
      partSys.startColor = new Color(0, 0.7490196f, 0.7254902f, 1);
      mana_color = new Color(0, 1, 0.9647059f, 1);
      mana_material.SetColor("_EmissionColor", mana_color * Mathf.Pow(2,2.5f));
      forceCol.enabled = false;

      protectLevel = protectLevelMax;
      regenTime = regenTime_N;
      jumpTime = jumpTime_N;
      health = healthMax;
      mana = manaMax;
      mana_intensity = Mathf.Pow(2, (mana / 8 - 2.5f));
   }

   private void Update()
   {
      MouseFace();
      legsTrans.position = transform.position + legsOffset;
   }


   private void FixedUpdate()
   {
      Move();
      Force();
      Jump();
   }




   //Передвижение
   public void MoveCalculate(Vector2 input)
   {
      inputMove += new Vector3(input.x, 0, input.y).normalized;
      legsAnim.SetBool("Move", true);
      idle = false;
      movementAngle = -Vector3.Angle(Vector3.back, inputMove);

      if (input.x < 0)
      {
         movementAngle = -movementAngle;
      }

      movementRotation = Quaternion.Euler(0, movementAngle, 0);
      movement = inputMove * speedWalk;
   }

   private void Move()
   {

      if (runAct && !idle)
      {
         movement = inputMove * speedRun;
         rb.MoveRotation(Quaternion.Slerp(transform.rotation, movementRotation, 14f * Time.deltaTime));
         playerAnim.SetBool("Run", true);
      }
      else
      {
         movement = inputMove * speedWalk;
         rb.MoveRotation(targetRotation);
         playerAnim.SetBool("Run", false);
      }

      if (jumpAct)
      {

      }
      else
      {

      }

      if (!idle)
      {
         rb.MovePosition(transform.position + movement);

         legsTrans.rotation = Quaternion.Slerp(legsTrans.rotation, movementRotation, 14f * Time.deltaTime);
         inputMove = Vector3.zero;
         idle = true;
      }
      else
      {
         legsAnim.SetBool("Move", false);
      }


   }


   //Прыжок
   public void Jump()
   {
      if (jumpAct)
      {
         //Отскок
         if (jumpBounce)
         {
            rb.AddForce(jumpVector);
            jumpBounce = false;
         }
         else
         {
            if (jumpTime > 0)
            {
               jumpTime -= Time.fixedDeltaTime;
            }
            else
            {
               rb.AddForce(-jumpVector/15);
               jumpFall = true;
            }

            if (ground && jumpFall)
            {
               Debug.Log("Suka");
               jumpAct = false;
               jumpFall = false;
               jumpBounce = true;
               jumpTime = jumpTime_N;
               jumpCD = jumpCD_N;
            }
         }
      }

      if (jumpCD > 0)
      {
         jumpCD -= Time.fixedDeltaTime;
      }


   }


   //Слежение за мышкой
   private void MouseFace()
   {
      playerPlane = new Plane(Vector3.up, transform.position);
      lookRay = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

      if (playerPlane.Raycast(lookRay, out hitDist))
      {
         targetPoint = lookRay.GetPoint(hitDist);
         targetRotation = Quaternion.LookRotation(transform.position - targetPoint);
         targetRotation.z = 0;
         targetRotation.x = 0;
         targetRotation = Quaternion.Slerp(transform.rotation, targetRotation, 14f * Time.deltaTime);
      }
   }



   
   //Сила
   private void Force() 
   {
      if (forceAct)
      {
         playerAnim.SetBool("Force", true);

         if (mana > 0)
         {
            forceCol.enabled = true;
            partSys.enableEmission = true;
            mana -= Time.deltaTime;
           // mana_intensity = Mathf.Pow(2,(mana / 8 - 2.5f));
            mana_material.SetColor("_EmissionColor", mana_color * mana_intensity);
         }
         else
         {
            forceCol.enabled = false;
            partSys.enableEmission = false;
         }
      }
      else
      {
         forceCol.enabled = false;
         partSys.enableEmission = false;
         playerAnim.SetBool("Force", false);
      }
   }

    public void SwitchForce()
    {
        fire = !fire;
        if (fire)
        {
            forceCol.tag = "fire";
            mana_color = new Color(1, 0.65f, 0, 1);
            mana_material.SetColor("_EmissionColor", mana_color * mana_intensity);
            partSys.startColor = new Color(1, 0.65f, 0, 1);
            for (int i = 0; i < lights_color.Length; i++)
            {
                lights_color[i].color = new Color(1, 0.65f, 0, 1);
            }
        }
        else
        {
            forceCol.tag = "ice";
            mana_color = new Color(0, 0.7490196f, 0.7254902f, 1);
            mana_material.SetColor("_EmissionColor", mana_color * mana_intensity);
            partSys.startColor = new Color(0, 0.7490196f, 0.7254902f, 1);
            for (int i = 0; i < lights_color.Length; i++)
            {
                lights_color[i].color = new Color(0, 0.7490196f, 0.7254902f, 1);
            }
        }
    }

    public void ReloadForce()
   {

   }




   //События и действия
   private void OnTriggerEnter(Collider interactive)
   {
      if (interactive.tag == "actionOff" || interactive.tag == "actionOn")
      {
         action = true;
         if (Input.GetKeyDown(KeyCode.E))
         {
            if (interactive.tag == "actionOn")
               interactive.tag = "actionOff";
            else
               interactive.tag = "actionOn";
         }

      }
   }

   private void OnTriggerExit(Collider interactive)
   {
      if (interactive.tag == "actionOff" || interactive.tag == "actionOn")
      {
         action = false;
      }
   }


   //Получение дамага
   public void TakeDamage(int damage)
   {
      health -= damage;
   
      if (health <= 0)
      {
         protectLevel--;
         health = healthMax;

         switch(protectLevel)
         {
             case 4:
                 protectLeavel_color = new Color(0,1,0);
                 break;
             case 3:
                 protectLeavel_color = new Color(1,1,0);
                 break;
             case 2:
                 protectLeavel_color = new Color(1,0.5f,0);
                 break;
             case 1:
                 protectLeavel_color = new Color(1,0,0);
                 break;

         }

         hp_material.SetColor("_EmissionColor", protectLeavel_color);
    
         if (protectLevel <= 0)
         {
            gameObject.SetActive(false);
         }
      }
   }


   private void OnCollisionEnter(Collision col)
   {
      if (col.gameObject.tag == "ground")
      {
         ground = true;
      }
   }

   private void OnCollisionExit(Collision col)
   {
      if (col.gameObject.tag == "ground")
      {
         ground = false;
      }
   }


   public void Save()
   {

   }
}
