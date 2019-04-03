using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

   [HideInInspector]
   public bool power = false;
   [HideInInspector]
   private bool blackout = false;
   public bool lockDoor;
   private bool open = false;
   private bool action = true;
   public bool horizontal;
   public Transform doorLeft;
   public Transform doorRight;
   private float doorLeftPos;
   private float doorRightPos;
   private float doorAddPos = 0;
   private float doorOpen = 0.7f;

   public GameObject player;
   public EnemyHelperAI eHelpAI;
   private float distBuf;
   private float dist;
   private float blackoutDist;
   private float overloadDist;

   public Renderer[] lamps;
   private Material[] material;
   private int lampsCount;

   private Color colorOn = new Color(0, 1, 0);
   private Color colorOff = new Color(1, 0, 0);
   private Color colorBlackout = new Color(0, 0, 0);



   void Start()
   {
      blackoutDist = eHelpAI.blackoutDist;
      overloadDist = eHelpAI.overloadDist;
      lampsCount = lamps.Length;
      material = new Material[lampsCount];

      for (int i = 0; i < lampsCount; i++)
         material[i] = lamps[i].material;


      if (this.tag == "electricityOn")
      {
         power = true;
         PowerOn();
      }

      if (this.tag == "electricityOff")
      {
         power = false;
         PowerOff();
      }

      if (horizontal)
      {
         doorLeftPos = doorLeft.position.x;
         doorRightPos = doorRight.position.x;
      }
      else
      {
         doorLeftPos = doorLeft.position.z;
         doorRightPos = doorRight.position.z;
      }
   }



   void FixedUpdate()
   {
      if (eHelpAI.night)
      {
         dist = blackoutDist + 1;

         for (int i = 0; i < eHelpAI.orblessCount; i++)
         {
            distBuf = (transform.position - eHelpAI.orblesses[i].transform.position).magnitude;
            if (dist > distBuf)
               dist = distBuf;
         }

         if (dist <= overloadDist)
         {
            blackout = false;
            PowerOn((1 / (dist+0.01f)) + Random.Range(-0.2f, 0f));
         }
         else if (dist <= blackoutDist)
         {
            if (!blackout)
            {
               PowerOff();
               blackout = true;
            }
         }
         else
         {
            if (blackout)
            {
               PowerOff();
               blackout = false;
            }
            else
            {
               Power();
            }
         }
      }
      else
      {
         Power();
      }

   }



   private void Power()
   {
      if (this.tag == "electricityOn" && !power)
      {
         PowerOn();
      }

      if (this.tag == "electricityOff")
      {
         PowerOff();
      }

      if(action)
      {
         CloseOpen();
      }

   }



   private void PowerOn()
   {
      power = true;
      if(lockDoor)
      {
         open = false;
         CloseOpen();
         for (int i = 0; i < lampsCount; i++)
            material[i].SetColor("_EmissionColor", colorOff * 100);
      }
      else
      {
         open = false;
         CloseOpen();
         for (int i = 0; i < lampsCount; i++)
            material[i].SetColor("_EmissionColor", colorOn * 5);
      }
   }



   private void PowerOn(float intensivity)
   {
      power = true;
      if (horizontal)
      {
         doorLeft.position = new Vector3(doorLeftPos - doorOpen * intensivity, doorLeft.position.y, doorLeft.position.z);
         doorRight.position = new Vector3(doorRightPos + doorOpen * intensivity, doorRight.position.y, doorRight.position.z);
      }
      else
      {
         doorLeft.position = new Vector3(doorLeft.position.x, doorLeft.position.y, doorLeftPos + doorOpen * intensivity);
         doorRight.position = new Vector3(doorRight.position.x, doorRight.position.y, doorRightPos - doorOpen * intensivity);
      }
      for (int i = 0; i < lampsCount; i++)
         material[i].SetColor("_EmissionColor", colorOn * intensivity * 5);
   }



   private void CloseOpen()
   {
      if (open)
      {
         doorAddPos = Mathf.Lerp(doorAddPos, doorOpen, 0.3f);
      }
      else
      {
         doorAddPos = Mathf.Lerp(doorAddPos, 0f, 0.3f);
      }

      if (horizontal)
      {
         doorLeft.position = new Vector3(doorLeftPos - doorAddPos, doorLeft.position.y, doorLeft.position.z);
         doorRight.position = new Vector3(doorRightPos + doorAddPos, doorRight.position.y, doorRight.position.z);
      }
      else
      {
         doorLeft.position = new Vector3(doorLeft.position.x, doorLeft.position.y, doorLeftPos + doorAddPos);
         doorRight.position = new Vector3(doorRight.position.x, doorRight.position.y, doorRightPos - doorAddPos);
      }

      if (doorAddPos == doorOpen || doorAddPos == 0f)
      {
         action = false;
      }
   }
   


   public void ByHand()
   {
      if (!power)
      {
         action = true;
         open = !open;
         CloseOpen();
         eHelpAI.Sound(transform.position, 10, player);
      }
   }



   private void OnTriggerEnter(Collider someone)
   {
      if (someone.tag == "Player" || someone.tag == "spider" || someone.tag == "orbless")
      {
         if (power && !lockDoor)
         {
            action = true;
            open = true;
            CloseOpen();
            eHelpAI.Sound(transform.position, 10, someone.gameObject);
            Debug.Log("Suka");
         }
      }
   }

   private void OnTriggerExit(Collider someone)
   {
      if (someone.tag == "Player" || someone.tag == "spider" || someone.tag == "orbless")
      {
         if (power && !lockDoor)
         {
            action = true;
            open = false;
            CloseOpen();
            eHelpAI.Sound(transform.position, 4, someone.gameObject);
            Debug.Log("Poka");
         }
      }
   }



   private void PowerOff()
   {
      power = false;
      action = true;
      this.tag = "door";
      for (int i = 0; i < lampsCount; i++)
            material[i].SetColor("_EmissionColor", colorBlackout);
   }
}
