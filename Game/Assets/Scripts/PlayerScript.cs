using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
   //Курсор
   [Header("Cursor")]
   public Texture2D[] cursor = new Texture2D[2];

   //Ввод
   public InputManager inputMan;
   private SubtitleGuiManager guiManager;

   //Костюм
   [Header("Suit")]
   public GameObject legs;
   private Transform listener;
   private Animator playerAnim;
   private Animator legsAnim;
   private Vector3 legsOffset;
   private Transform legsTrans;
   public Light[] lights_suit;
   [HideInInspector]
   public float[] lights_suit_intens;
   public Light light_snow;
   private float light_snow_intens;
   public Transform cameraTrans;
   private Color[] suit_lightColor = new Color[2];
   private BoxCollider coll;


   //События и действия
   [HideInInspector]
   public bool idle = false;
   [HideInInspector]
   public bool action = false;
   private bool actComplete = false;
   [HideInInspector]
   public bool actEasyHappen = false;
   [HideInInspector]
   public bool doing = false;
   private float actHardTime;
   private float actEasyTime;
   private Quaternion lookAct;
   private float angleAct;
   private const float actHardTime_N = 4.0f;
   private const float actEasyTime_N = 0.4f;
   private GameObject actObject;
   private bool actBoolBuf;
   [HideInInspector]
   public bool suitOff;


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
   public bool forceScare = false;
   [HideInInspector]
   public bool forceAct = false;
   private bool forceActNow = false;
   [HideInInspector]
   public bool forcePrep = false;
   private float mana;
   [HideInInspector]
   public byte forceType = 0;
   public Material mana_material;
   private Color mana_color;
   [HideInInspector]
   public float mana_intensity = 0.0f;
   private Color[] force_lightColor = new Color[2];
   [HideInInspector]
   public bool forceSwitch = false;
   private float switchTime;
   private float switchTime_N = 1.0f;
   private string[] forceType_string = new string[2];



   //Звуки
   [FMODUnity.EventRef]
   public string[] forceSounds = new string[2];
   private FMOD.Studio.EventInstance[] forceSoundsInst = new FMOD.Studio.EventInstance[2];



   //Здоровье
   [HideInInspector]
   public int health;
   private const int healthMax = 4;
   [HideInInspector]
   public Material hp_material;
   [HideInInspector]
   public float hp_intensity = 1.0f;
   private Color hp_color;


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
   private float runSoundCD_N = 0.2f;
   private float walkSoundCD_N = 0.3f;
   private float moveSoundCD;
   private Rigidbody rb;
   private Ray lookRay;
   [HideInInspector]
   public bool runAct = false;


   //Звуки
   public EnemyHelperAI enemyHelpAI;


    //Прыжок
    private Vector3 jumpVector = new Vector3(0,800,0);
   [HideInInspector]
   public bool jumpAct = false;
   private bool jumpBounce = true;
   private bool jumpFall = false;
   private bool ground = true;
   [HideInInspector]
   public float jumpCD = 0;
   private const float jumpCD_N = 0.5f;
   private float jumpTime;
   private const float jumpTime_N = 0.2f;

   [ContextMenu("AutoFill")]
   public void Fill()
   {
      inputMan = GameObject.Find("GameManager").GetComponent<InputManager>();
      cameraTrans = GameObject.Find("Camera Target").transform;
      legs = GameObject.Find("Legs");
   }



   private void Awake()
   {

      coll = GetComponent<BoxCollider>();
      force_particle = force.GetComponent<ParticleSystem>();
      legsTrans = legs.GetComponent<Transform>();
      legsAnim = legs.GetComponent<Animator>();
      legsOffset = legsTrans.position - transform.position;
      playerAnim = GetComponent<Animator>();
      rb = GetComponent<Rigidbody>();
      cameraTrans.position = new Vector3(transform.position.x, 0 , transform.position.z);

      force_lightColor[0] = new Color(0, 0.6f, 0.7254902f);
      force_lightColor[1] = new Color(0.8f, 0.5f, 0);
      suit_lightColor[0] = new Color(0.4f, 0.9f, 1.0f);
      suit_lightColor[1] = new Color(1.0f, 0.86f, 0.63f);
      forceType_string[0] = "ice";
      forceType_string[1] = "fire";
      forceType = 1;
      mana_intensity = 3.5f;
      hp_intensity = 1.0f;

      LightSuit();
      for (int i = 0; i < lights_suit.Length; i++)
      {
         lights_suit_intens[i] = lights_suit[i].intensity;
      }
   }

   private void Start()
   {
      guiManager = FindObjectOfType<SubtitleGuiManager>();
      actHardTime = actHardTime_N;
      actHardTime = actHardTime_N;
      actEasyTime = actEasyTime_N;
      jumpTime = jumpTime_N;
      health = healthMax;
      mana = manaMax;

      forceSoundsInst[0] = FMODUnity.RuntimeManager.CreateInstance(forceSounds[0]);
      forceSoundsInst[1] = FMODUnity.RuntimeManager.CreateInstance(forceSounds[1]);

      force_particle.enableEmission = false;
      forcePrep_particle.enableEmission = false;
      hp_color = new Color(0, 1, 0, 1);
   


      listener = GameObject.Find("Listener").transform;
   }

   private void Update()
   {
      if (!inputMan.cutScene)
      {
         MouseFace();
      }

      legsTrans.position = transform.position + legsOffset;
      listener.position = transform.position;

      if (action)
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

            if(moveSoundCD>0)
            {
               moveSoundCD -= Time.fixedDeltaTime;
            }
            else
            {
               moveSoundCD = runSoundCD_N;
               enemyHelpAI.Sound(transform.position, 20, this.gameObject);
            }
         }
         else
         {
            movement = inputMove * speedWalk;
            if (jumpAct)
               rb.MoveRotation(Quaternion.Slerp(transform.rotation, movementRotation, 14f * Time.deltaTime));
            else
            {
               rb.MoveRotation(targetRotation);

               if (moveSoundCD > 0)
               {
                  moveSoundCD -= Time.fixedDeltaTime;
               }
               else
               {
                  moveSoundCD = walkSoundCD_N;
                  enemyHelpAI.Sound(transform.position, 8, this.gameObject);
               }
            }
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
         moveSoundCD = 0;
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
         
         legsAnim.SetBool("Jump", true);
         if (jumpBounce)
         {
            rb.AddForce(jumpVector);
            jumpBounce = false;
            enemyHelpAI.Sound(transform.position, 20, this.gameObject);
            //Debug.Log("Отпрыгнули");
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

            if (jumpFall)
            {
               jumpAct = false;
               jumpFall = false;
               jumpBounce = true;
               jumpTime = jumpTime_N;
               jumpCD = jumpCD_N;
               legsAnim.SetBool("Jump", false);
               enemyHelpAI.Sound(transform.position, 20, this.gameObject);
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

         if (mana > 0)
         {
            forceScare = true;

            if (forceAct)
            {
               if(!forceActNow)
               {
                  force_Col.enabled = true;
                  force_particle.enableEmission = true;
                  forcePrep_particle.enableEmission = false;
                  force_light_intens = 1.8f; 
                  light_snow_intens = 8;
                  forceActNow = true;
                  forceSoundsInst[forceType].start();
               }
                  mana -= Time.deltaTime;
                  enemyHelpAI.Sound(transform.position, 40, this.gameObject);
            }
            else
            {
               forceSoundsInst[forceType].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
               forceActNow = false;
               force_Col.enabled = false;
               force_particle.enableEmission = false;
               //enemyHelpAI.Sound(transform.position, 8, this.gameObject);
               forcePrep_particle.enableEmission = true;
               mana -= Time.deltaTime * 0.1f;
               force_light_intens = 1.8f;
               if (suitOff)
                  light_snow_intens = 2;
               else
                  light_snow_intens = 4;
            }

         }
         else
         {
            force_Col.enabled = false;
            force_particle.enableEmission = false;
            forceSoundsInst[forceType].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            forceActNow = false;
            forceScare = false;
            forcePrep_particle.enableEmission = false;
            force_light_intens = 0;
            if (suitOff)
               light_snow_intens = 0;
            else
               light_snow_intens = 4;
         }

      }
      else
      {
         forceSoundsInst[forceType].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
         forceActNow = false;
         forceScare = false;
         coll.size = new Vector3(0.8f, 1.5f, 0.5f);
         coll.center = new Vector3(0.0f, 0.25f, 0.0f);
         forcePrep_particle.enableEmission = false;
         force_Col.enabled = false;
         force_particle.enableEmission = false;
         forcePrep_particle.enableEmission = false;
         playerAnim.SetBool("Force", false);
         force_light_intens = 0;
         if (suitOff)
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
      forceSoundsInst[forceType].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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
      hp_material.SetColor("_EmissionColor", hp_color * hp_intensity);

      for (int i = 0; i < lights_suit.Length; i++)
      {
         lights_suit[i].color = suit_lightColor[forceType];
      }
      if(suitOff)
      {
         lights_suit[0].color = new Color(1, 1, 1);
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
      if ((interactive.tag == "engine" && !suitOff) || interactive.tag == "computer" || interactive.tag == "door" || interactive.gameObject.layer == 11 || interactive.tag == "terminal" || interactive.tag == "easyEngine")
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
            actEasyHappen = false;
            actComplete = false;
            actHardTime = actHardTime_N;
            actEasyTime = actEasyTime_N;
            playerAnim.SetBool("ActionHard", false);
         }
      }
   }


   private void OnTriggerExit(Collider interactive)
   {
      action = false;
      actEasyHappen = false;
      actComplete = false;
      actHardTime = actHardTime_N;
      actEasyTime = actEasyTime_N;
      playerAnim.SetBool("ActionHard", false);
      playerAnim.SetBool("ActionEasy", false);

   }


   //Действия игрока с предметами
   public void Act()
   {
      if (doing && actObject != null)
      {
         //Включение двигателя(зажать)
         if (actObject.tag == "engine")
         {
            if (actHardTime > 0)
            {
               //Debug.Log("Двигатель включается")
               actHardTime -= Time.deltaTime;
               playerAnim.SetBool("ActionHard", true);
            }
            else
            {
               if (!actComplete)
               {
                  //Debug.Log("Вкл/Выкл двигателя + остановка действия")
                  actObject.GetComponent<Engine>().power = !actObject.GetComponent<Engine>().power;
                  playerAnim.SetBool("ActionHard", false);
                  playerAnim.SetBool("ActionEasy", false);
                  actComplete = true;
               }
            }
         }

         //Открытие дверей(одно нажатие)
         if (actObject.tag == "door" && !actComplete)
         {
            if (!actEasyHappen)
            {
               playerAnim.SetBool("ActionEasy", true);
               actObject.GetComponent<Door>().ByHand();
               actEasyHappen = true;
            }

            if (actEasyTime > 0)
            {
               actEasyTime -= Time.deltaTime;
            }
            else
            {
               actEasyHappen = false;
               actComplete = true;
               actEasyTime = actEasyTime_N;
               playerAnim.SetBool("ActionEasy", false);
            }
         }

         if (actObject.tag == "terminal" && !actComplete)
         {
            if (!actEasyHappen)
            {
               playerAnim.SetBool("ActionEasy", true);
               actObject.GetComponent<Terminal>().Switch();
               actEasyHappen = true;
            }

            if (actEasyTime > 0)
            {
               actEasyTime -= Time.deltaTime;
            }
            else
            {
               actEasyHappen = false;
               actComplete = true;
               actEasyTime = actEasyTime_N;
               playerAnim.SetBool("ActionEasy", false);
            }
         }

         if (actObject.tag == "easyEngine" && !actComplete)
         {
            if (!actEasyHappen)
            {
               playerAnim.SetBool("ActionEasy", true);
               actObject.GetComponent<EasyEngine>().PowerOn();
               actEasyHappen = true;
            }

            if (actEasyTime > 0)
            {
               actEasyTime -= Time.deltaTime;
            }
            else
            {
               actEasyHappen = false;
               actComplete = true;
               actEasyTime = actEasyTime_N;
               playerAnim.SetBool("ActionEasy", false);
            }
         }

         if (actObject.tag == "AudioRecord" && !actComplete)
         {
            if (!actEasyHappen)
            {
               string path = ("event:/AudioRecordsAndNotes/" + actObject.name);
               AudioRecordsAndNotes.AudioRecords_Script.AddAudioRecord(path);
               playerAnim.SetBool("ActionEasy", true);
               actEasyHappen = true;
               Destroy(actObject.gameObject);
            }

            if (actEasyTime > 0)
            {
               actEasyTime -= Time.deltaTime;
            }
            else
            {
               actEasyHappen = false;
               actComplete = true;
               actEasyTime = actEasyTime_N;
               playerAnim.SetBool("ActionEasy", false);
            }
         }

         if (actObject.tag == "Note" && !actComplete)
         {
            if (!actEasyHappen && !AudioRecordsAndNotes.Notes_Script.cheking)
            {
               string path = ("event:/AudioRecordsAndNotes/OpenNote");
               AudioRecordsAndNotes.Notes_Script.OpenNote(path, actObject);
               playerAnim.SetBool("ActionEasy", true);
               actEasyHappen = true;
            }

            if (actEasyTime > 0)
            {
               actEasyTime -= Time.deltaTime;
            }
            else
            {
               actEasyHappen = false;
               actComplete = true;
               actEasyTime = actEasyTime_N;
               playerAnim.SetBool("ActionEasy", false);
            }
         }

         if (actObject.tag == "computer" && !actComplete)
         {
            if (!actEasyHappen)
            {
               playerAnim.SetBool("ActionEasy", true);
               actObject.GetComponent<Computer>().Unlock();
               actEasyHappen = true;
            }

            if (actEasyTime > 0)
            {
               actEasyTime -= Time.deltaTime;
            }
            else
            {
               actEasyHappen = false;
               actComplete = true;
               actEasyTime = actEasyTime_N;
               playerAnim.SetBool("ActionEasy", false);
            }
         }
      }
      else
      {
         //Debug.Log("Обнуление переменных")
         actEasyHappen = false;
         actComplete = false;
         actHardTime = actHardTime_N;
         actEasyTime = actEasyTime_N;
         playerAnim.SetBool("ActionHard", false);
         playerAnim.SetBool("ActionEasy", false);
      }
   }


   //Получение дамага
   public void TakeDamage(int damage)
   {
      health -= damage;

      if (health <= 0)
      {
         Death(); 
      }

      switch (health)
      {
         case 4:
            hp_color = new Color(0, 1, 0);
            break;
         case 3:
            hp_color = new Color(1, 1, 0);
            break;
         case 2:
            hp_color = new Color(1, 0.5f, 0);
            break;
         case 1:
            hp_color = new Color(1, 0, 0);
            break;

      }
      LightSuit();
   }


   public void Death()
   {
      inputMan.death = true;
      inputMan.game = false;
      guiManager.SetText("Ты умер. R - ПЕРЕЗАГРУЗКА");
      this.gameObject.SetActive(false);
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
