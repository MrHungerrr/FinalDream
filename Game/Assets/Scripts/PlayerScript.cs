using UnityEngine;

public class PlayerScript : MonoBehaviour
{
   //Курсор
   [Header("Cursor")]
   public Texture2D[] cursor = new Texture2D[2];


   //Костюм
   [Header("Suit")]
   public GameObject legs;
   private Animator playerAnim;
   private Animator legsAnim;
   private Vector3 legsOffset;
   private Transform legsTrans;
   public Light[] lights_suit = new Light[3];
   [HideInInspector]
   public float[] lights_suit_intens = new float[3];
   public Light light_snow;
   private float light_snow_intens;
   public Transform cameraTrans;
   private Color[] suit_lightColor = new Color[2];
   private BoxCollider coll;


   //События и действия
   private bool idle = false;
   [HideInInspector]
   public bool action = false;
   private bool actionStart = false;
   [HideInInspector]
   public bool doing = false;
   private float actTime;
   private Quaternion lookAct;
   private float angleAct;
   private const float actTime_N = 0.4f;
   private GameObject actObject;
   [HideInInspector]
   public bool sonarDis;


   //Сила и мана
   [Header("Force")]
   public Light[] lights_force = new Light[2];
   public GameObject force;
   public Light force_light;
   private float force_light_intens;
   public  ParticleSystem forcePrep_particle;
   private ParticleSystem force_particle;
   private const float manaMax = 40;
   public Collider force_Col;
   [HideInInspector]
   public bool forceAct = false;
   [HideInInspector]
   public bool forcePrep = false;
   private float mana;
   private byte forceType = 0;
   public Material mana_material;
   [HideInInspector]
   public Color mana_color;
   [HideInInspector]
   public float mana_intensity = 6.0f;
   private Color[] force_lightColor = new Color[2];
   [HideInInspector]
   public bool forceSwitch = false;
   private float switchTime;
   private float switchTime_N = 1.0f;
   private string[] forceType_string = new string[2];


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
   [HideInInspector]
   public float hp_intensity;
   private Color protectLevel_color;
 //  private float hp_intensity;


   //Движение
   private const float speedWalk = 0.1f;
   private const float speedRun = 0.15f;
   private float hitDist = 0.0f;
   private float movementAngle;
   private Plane playerPlane;
   private Quaternion movementRotation;
   private Quaternion targetRotation = new Quaternion(0,0,0,1);
   [HideInInspector]
   public Vector3 targetPoint;
   private Vector3 inputMove;
   private Vector3 movement;
   private Rigidbody rb;
   private Ray lookRay;
   [HideInInspector]
   public bool runAct = false;


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
      coll = GetComponent<BoxCollider>();
      force_particle = force.GetComponent<ParticleSystem>();
      legsTrans = legs.GetComponent<Transform>();
      legsAnim = legs.GetComponent<Animator>();
      legsOffset = legsTrans.position;
      playerAnim = GetComponent<Animator>();
      rb = GetComponent<Rigidbody>();
      cameraTrans.position = transform.position;
   }

   private void Start()
   {
      protectLevel = protectLevelMax;
      regenTime = regenTime_N;
      jumpTime = jumpTime_N;
      health = healthMax;
      mana = manaMax;
      force_lightColor[0] = new Color(0, 0.6f, 0.7254902f, 1);
      force_lightColor[1] = new Color(0.8f, 0.5f, 0, 1);
      suit_lightColor[0] = new Color(0.4f, 0.9f, 1.0f, 1);
      suit_lightColor[1] = new Color(1.0f, 0.86f, 0.63f, 1);
      forceType_string[0] = "ice";
      forceType_string[1] = "fire";
      forceType = 0;

      force_particle.enableEmission = false;
      forcePrep_particle.enableEmission = false;
      protectLevel_color = new Color(0, 1, 0, 1);
      hp_material.SetColor("_EmissionColor", protectLevel_color);
   
      LightSuit();
      for (int i = 0; i < lights_suit.Length; i++)
      {
         lights_suit_intens[i] = lights_suit[i].intensity;
      }

   }

   private void Update()
   {
      MouseFace();
      legsTrans.position = transform.position + legsOffset;
      
      if(doing)
      {
         Act();
      }
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
            cameraTrans.position = transform.position + new Vector3(inputMove.x, 0, inputMove.z).normalized * 3;
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
            cameraTrans.position = transform.position;

         }



         rb.MovePosition(transform.position + movement);

         legsTrans.rotation = Quaternion.Slerp(legsTrans.rotation, movementRotation, 14f * Time.deltaTime);
         inputMove = Vector3.zero;
         idle = true;
      }
      else
      {
         cameraTrans.position = transform.position;
         rb.MoveRotation(targetRotation);
         legsAnim.SetBool("Move", false);
         playerAnim.SetBool("Run", false);
      }


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
         coll.size = new Vector3(0.8f, 1.5f, 0.75f);
         coll.center = new Vector3(0.0f, 0.25f, -0.15f);
         cameraTrans.position = transform.position - (new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(targetPoint.x, 0, targetPoint.z)).normalized*3;
   

         if (!forceAct)
         {
            if (mana > 0)
            {
               forcePrep_particle.enableEmission = true;
               force_light_intens = 1.2f;
               if (sonarDis)
                  light_snow_intens = 2;
               else
                  light_snow_intens = 4;
            }
            else
            {
               forcePrep_particle.enableEmission = false;
               force_light_intens = 0;
               if (sonarDis)
                  light_snow_intens = 0;
               else
                  light_snow_intens = 4;
            }
         }
         else
         {
            forcePrep_particle.enableEmission = false;
            force_light_intens = 0;
         }


         if (forceAct && mana > 0)
         {
            light_snow_intens = 8;
            force_Col.enabled = true;
            force_particle.enableEmission = true;
            mana -= Time.deltaTime;
         }
         else
         {
            force_particle.enableEmission = false;
            force_Col.enabled = false;
            if (sonarDis)
               light_snow_intens = 0;
            else
               light_snow_intens = 4;

         }

      }
      else
      {
         coll.size = new Vector3(0.8f, 1.5f, 0.5f);
         coll.center = new Vector3(0.0f, 0.25f, 0.0f);
         forcePrep_particle.enableEmission = false;
         force_Col.enabled = false;
         force_particle.enableEmission = false;
         forcePrep_particle.enableEmission = false;
         playerAnim.SetBool("Force", false);
         force_light_intens = 0;
         if (sonarDis)
            light_snow_intens = 0;
         else
            light_snow_intens = 4;
      }

      light_snow.intensity = Mathf.Lerp(light_snow.intensity, light_snow_intens, 0.1f);
      force_light.intensity = Mathf.Lerp(force_light.intensity, force_light_intens, 0.3f);

      if (forceSwitch)
      {
         if(switchTime>0)
         {
            switchTime -= Time.fixedDeltaTime;
         }
         else
         {
            LightSuit();
            forceSwitch = false;
            playerAnim.SetBool("Switch", false);
         }
      }
   }


   public void SwitchForce()
   {
      if (forceType == 0)
      {
         forceType = 1;
      }
      else
      {
         forceType = 0;
      }
      forceSwitch = true;
      playerAnim.SetBool("Switch", true);
      switchTime = switchTime_N;
   }


   public void LightSuit()
   {
      Cursor.SetCursor(cursor[forceType], Vector2.zero, CursorMode.Auto);
      force_Col.tag = forceType_string[forceType];
      mana_color = force_lightColor[forceType];
      mana_material.SetColor("_EmissionColor", mana_color * mana_intensity);
      force_particle.startColor = mana_color;
      forcePrep_particle.startColor = mana_color;
      if (sonarDis)
         hp_material.SetColor("_EmissionColor", protectLevel_color * hp_intensity);
      else
         hp_material.SetColor("_EmissionColor", protectLevel_color);

      for (int i = 0; i < lights_suit.Length; i++)
      {
         lights_suit[i].color = suit_lightColor[forceType];
      }
      for (int i = 0; i < lights_force.Length; i++)
      {
         lights_force[i].color = mana_color;
      }
   }


   public void ReloadForce()
   {

   }


   //События и действия
   private void OnTriggerStay(Collider interactive)
   {
      if (interactive.tag == "actionOff" || interactive.tag == "actionOn")
      {

         lookAct = Quaternion.LookRotation(transform.position - interactive.transform.position);
         angleAct = Quaternion.Angle(transform.rotation, lookAct);

         if (angleAct < 60)
         {
            action = true;
            actObject = interactive.gameObject;
         }
         else
         {
            action = false;
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


   public void Act()
   {
      if ((actObject.tag == "actionOff" || actObject.tag == "actionOn") && actionStart)
      {
         playerAnim.SetBool("Action", true);
         if (actObject.tag == "actionOn")
            actObject.tag = "actionOff";
         else
            actObject.tag = "actionWantOn";
         actionStart = false;
      }
      
      if(actTime>0)
      {
         actTime -= Time.deltaTime;
      }
      else
      {
         doing = false;
         actTime = actTime_N;
         actionStart = true;
         playerAnim.SetBool("Action", false);
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
                 protectLevel_color = new Color(0,1,0);
                 break;
             case 3:
                 protectLevel_color = new Color(1,1,0);
                 break;
             case 2:
                 protectLevel_color = new Color(1,0.5f,0);
                 break;
             case 1:
                 protectLevel_color = new Color(1,0,0);
                 break;

         }

         LightSuit();
    
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
