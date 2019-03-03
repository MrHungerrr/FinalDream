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
   public Light light_camera;


   //События и действия
   private bool idle = false;
   private bool action = false;


   //Сила и мана
   [Header("Force")]
   public GameObject force;
   public  ParticleSystem forcePrep_Particle;
   private ParticleSystem force_Particle;
   private const float manaMax = 40;
   public Collider force_Col;
   [HideInInspector]
   public bool forceAct = false;
   [HideInInspector]
   public bool forcePrep = false;
   [HideInInspector]
   public bool forceSpec = false;
   private float mana;
   private bool fire;
   public Material mana_Material;
   private Color mana_Color;
   private float mana_Intensity;


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
   public Material hp_Material;
   private Color protectLevel_Color;
 //  private float hp_intensity;


   //Движение
   private const float speedWalk = 0.1f;
   private const float speedRun = 0.15f;
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
   private bool jumpFall = false;
   private bool ground = true;
   [HideInInspector]
   public float jumpCD = 0;
   private const float jumpCD_N = 1.0f;
   private float jumpTime;
   private const float jumpTime_N = 0.85f;




   private void Awake()
   {
      force_Particle = force.GetComponent<ParticleSystem>();
      legsTrans = legs.GetComponent<Transform>();
      legsAnim = legs.GetComponent<Animator>();
      legsOffset = legsTrans.position;
      playerAnim = GetComponent<Animator>();
      rb = GetComponent<Rigidbody>();
   }

   private void Start()
   {
      force_Particle.enableEmission = false;
      forcePrep_Particle.enableEmission = false;
      protectLevel_Color = new Color(0, 1, 0, 1);
      hp_Material.SetColor("_EmissionColor",protectLevel_Color);
      force_Particle.startColor = new Color(0, 0.7490196f, 0.7254902f, 1);
      mana_Color = new Color(0, 1, 0.9647059f, 1);
      mana_Material.SetColor("_EmissionColor", mana_Color * Mathf.Pow(2,2.5f));
      force_Col.enabled = false;

      protectLevel = protectLevelMax;
      regenTime = regenTime_N;
      jumpTime = jumpTime_N;
      health = healthMax;
      mana = manaMax;
      mana_Intensity = Mathf.Pow(2, (mana / 8 - 2.5f));
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
      if (!idle)
      {
         if (runAct)
         {
            movement = inputMove * speedRun;
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, movementRotation, 14f * Time.deltaTime));
            playerAnim.SetBool("Run", true);
         }
         else
         {
            movement = inputMove * speedWalk;
            if (jumpAct)
               rb.MoveRotation(Quaternion.Slerp(transform.rotation, movementRotation, 14f * Time.deltaTime));
            else
               rb.MoveRotation(targetRotation);
            playerAnim.SetBool("Run", false);
         }



         rb.MovePosition(transform.position + movement);

         legsTrans.rotation = Quaternion.Slerp(legsTrans.rotation, movementRotation, 14f * Time.deltaTime);
         inputMove = Vector3.zero;
         idle = true;
      }
      else
      {
         rb.MoveRotation(targetRotation);
         legsAnim.SetBool("Move", false);
         playerAnim.SetBool("Run", false);
      }

      Debug.Log(transform.rotation.y);


   }



   //Прыжок
   public void Jump()
   {
      if (jumpAct)
      {
         //Отскок
         legsAnim.SetBool("Jump", true);
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
               jumpAct = false;
               jumpFall = false;
               jumpBounce = true;
               jumpTime = jumpTime_N;
               jumpCD = jumpCD_N;
               legsAnim.SetBool("Jump", false);
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
      //Подготовка
      if (forcePrep)
      {
         playerAnim.SetBool("Force", true);
         
         if(!forceAct && !forceSpec)
         {
            if (mana > 0)
               forcePrep_Particle.enableEmission = true;
            else
               forcePrep_Particle.enableEmission = false;
         }
         else
            forcePrep_Particle.enableEmission = false;


         if (forceAct && mana > 0)
         {
            force_Col.enabled = true;
            force_Particle.enableEmission = true;
            mana -= Time.deltaTime;
           // mana_intensity = Mathf.Pow(2,(mana / 8 - 2.5f));
            mana_Material.SetColor("_EmissionColor", mana_Color * mana_Intensity);
         }
         else
         {
            force_Col.enabled = false;
            force_Particle.enableEmission = false;
            
         }

         //СпешлУдар
         if (forceSpec && mana>5)
         {
            if(fire)
            {

            }
         }
      }
      else
      {
         force_Col.enabled = false;
         force_Particle.enableEmission = false;
         forcePrep_Particle.enableEmission = false;
         playerAnim.SetBool("Force", false);
      }
   }

    public void SwitchForce()
    {
        fire = !fire;
        if (fire)
        {
            force_Col.tag = "fire";
            mana_Color = new Color(1, 0.65f, 0, 1);
            mana_Material.SetColor("_EmissionColor", mana_Color * mana_Intensity);
            force_Particle.startColor = new Color(1, 0.65f, 0, 1);
            for (int i = 0; i < lights_color.Length; i++)
            {
                lights_color[i].color = new Color(1, 0.65f, 0, 1);
            }
        }
        else
        {
            force_Col.tag = "ice";
            mana_Color = new Color(0, 0.7490196f, 0.7254902f, 1);
            mana_Material.SetColor("_EmissionColor", mana_Color * mana_Intensity);
            force_Particle.startColor = new Color(0, 0.7490196f, 0.7254902f, 1);
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
                 protectLevel_Color = new Color(0,1,0);
                 break;
             case 3:
                 protectLevel_Color = new Color(1,1,0);
                 break;
             case 2:
                 protectLevel_Color = new Color(1,0.5f,0);
                 break;
             case 1:
                 protectLevel_Color = new Color(1,0,0);
                 break;

         }

         hp_Material.SetColor("_EmissionColor", protectLevel_Color);
    
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
