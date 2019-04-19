using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class InputManager : MonoBehaviour
{ 
   
   [HideInInspector]
   public bool game = true;
   [HideInInspector]
   public bool cutScene = false;
   public bool death = false;
   private Vector2 inputMove;
   private PlayerScript pScript;
   public LevelManager levelMan;

   //private ParticleSystem.VelocityOverLifetimeModule vel;
   //private ParticleSystem.ShapeModule shape;
   private void Start()
   {
      pScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
   }


   void Update()
   {
      if (game)
      {
            GameInput();
      }
      else
         MenuInput();
   }


   void FixedUpdate()
   {
      if (game && !cutScene)
         GameFixInput();
   }


   private void GameFixInput()
   {
      // Передвижение
      inputMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

      if (inputMove != Vector2.zero)
      {
         pScript.MoveCalculate(inputMove);
      }

      //Сила
      if (Input.GetMouseButton(1) && !pScript.jumpAct && !pScript.runAct && !pScript.forceSwitch && !pScript.doing)
      {
         pScript.forcePrep = true;

       /*  if (Input.GetMouseButton(0))
         {
            pScript.forceAct = true;
         }
         else
         {
            pScript.forceAct = false;
         }
         */
      }
      else
      {
         pScript.forceAct = false;
         pScript.forcePrep = false;
      }
   }

   private void GameInput()
   {
      if (!cutScene)
      {
         //Смена силы
         /* if (Input.GetKeyDown(KeyCode.Q) && !pScript.forceSwitch && !pScript.doing)
          {
             pScript.SwitchForce();
          }
          */

         //Действие
         if (Input.GetKey(KeyCode.E) && pScript.action && !pScript.forcePrep && !pScript.jumpAct && !pScript.forceSwitch)
         {
            pScript.doing = true;
         }
         else if (!pScript.actEasyHappen)
         {
            pScript.doing = false;
         }

         /*
                  //Перезарядка кристалла
                  if (Input.GetKeyDown(KeyCode.R))
                  {
                     pScript.ReloadForce();
                  }
*/

         //бег
         if (Input.GetKey(KeyCode.LeftShift) && !pScript.forcePrep && !pScript.jumpAct && !pScript.forceSwitch && !pScript.doing)
         {
            pScript.runAct = true;
         }
         else
         {
            pScript.runAct = false;
         }


         //Способность костюма
         if (Input.GetKeyDown(KeyCode.Space) && (pScript.jumpCD <= 0) && !pScript.forcePrep && !pScript.doing)
         {
            pScript.jumpAct = true;
         }


         //Сохранение
         /* if (Input.GetKey(KeyCode.F) && !player.forcePrep && !player.forceSwitch && inputMove == Vector2.zero && !player.jumpAct && !player.doing)
          {
             player.Save();
          }

          //Пейджер
          if (Input.GetKeyDown(KeyCode.Tab))
          {

          }
          */

         if (Input.GetKeyDown(KeyCode.Z))
         {
            int k = 0;
            AudioRecordsAndNotes.AudioRecords_Script.AudioListening(k);
         }
      }
      else
      {
         if (Input.GetKeyDown(KeyCode.Space))
         {
            levelMan.CutSceneEnd();
         }
      }
      //Меню
      if (Input.GetKeyDown(KeyCode.Escape))
      {
         if (AudioRecordsAndNotes.Notes_Script.cheking)
         {
            AudioRecordsAndNotes.Notes_Script.CloseNote("event:/AudioRecordsAndNotes/CloseNote");
         }
         else
         {
            Time.timeScale = 0;
            game = false;
         }
      }



   }

   private void MenuInput()
   {
      if (death)
      {
         if (Input.GetKeyDown(KeyCode.R))
         {
            Application.LoadLevel(0);
         }
      }
      else
      {
         if (Input.GetKeyDown(KeyCode.Escape))
         {
            Time.timeScale = 1;
            game = true;
         }

         if (Input.GetKeyDown(KeyCode.R))
         {
            Application.LoadLevel(0);
            Time.timeScale = 1;
            game = true;
         }
      }
   }
}
