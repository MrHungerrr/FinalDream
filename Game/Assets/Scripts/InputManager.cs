using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class InputManager : MonoBehaviour
{ 
   
   [HideInInspector]
   public bool game = true;
   private Vector2 inputMove;
   public PlayerScript player;

    //private ParticleSystem.VelocityOverLifetimeModule vel;
    //private ParticleSystem.ShapeModule shape;

   void Update()
   {
      if (game)
         GameInput();
      else
         MenuInput();
   }

   void FixedUpdate()
   {
      if (game)
         GameFixInput();
   }


   private void GameFixInput()
   {
      // Передвижение
      inputMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

      if (inputMove != Vector2.zero)
      {
         player.MoveCalculate(inputMove);
      }

      //Сила
      if (Input.GetMouseButton(1) && !player.jumpAct && !player.runAct && !player.forceSwitch && !player.doing)
      {
         player.forcePrep = true;

         if (Input.GetMouseButton(0))
         {
            player.forceAct = true;
         }
         else
         {
            player.forceAct = false;
         }

      }
      else
      {
         player.forceAct = false;
         player.forcePrep = false;
      }
   }

   private void GameInput()
   {

      //Смена силы
      if (Input.GetKeyDown(KeyCode.Q) && !player.forceSwitch && !player.doing)
      {
         player.SwitchForce();
      }


      //Действие
      if (Input.GetKey(KeyCode.E) && player.action && !player.forcePrep && !player.jumpAct && !player.forceSwitch)
      {
         player.doing = true;
      }
      else if (!player.actEasyHappen)
      {
         player.doing = false;
      }


      //Перезарядка кристалла
      if (Input.GetKeyDown(KeyCode.R))
      {
         player.ReloadForce();
      }


      //бег
      if (Input.GetKey(KeyCode.LeftShift) && !player.forcePrep && !player.jumpAct && !player.forceSwitch && !player.doing)
      {
         player.runAct = true;
      }
      else
      {
         player.runAct = false;
      }


      //Способность костюма
      if (Input.GetKeyDown(KeyCode.Space) && (player.jumpCD <= 0) && !player.forcePrep && !player.doing)
      { 
         player.jumpAct = true;
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



      //Меню
      if (Input.GetKeyDown(KeyCode.Escape))
      {
            if (AudioRecordsAndNotes.Notes_Script.cheking)
            {
                AudioRecordsAndNotes.Notes_Script.CloseNote("event:/AudioRecordsAndNotes/CloseNote");
            }
        }

      if (Input.GetKeyDown(KeyCode.Z))
      {
            int k = 0;
            AudioRecordsAndNotes.AudioRecords_Script.AudioListening(k);
      }
   } 

   private void MenuInput()
   {
     
   }
}
