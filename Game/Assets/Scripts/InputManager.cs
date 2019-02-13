using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
   [HideInInspector]
   public bool game = true;
   private Vector2 inputMove;
   public PlayerScript player;
   public ParticleSystem partSosi;

   //private ParticleSystem.VelocityOverLifetimeModule vel;
   //private ParticleSystem.ShapeModule shape;


   void Start()
   {
   }


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

         var vel = partSosi.velocityOverLifetime;
         vel = partSosi.velocityOverLifetime;
         vel.x = 5 * -inputMove.x;
         vel.y = 2 * -inputMove.y;

        if (inputMove.y == -1)
            {
                partSosi.startLifetime = 4.2f;
            }
        else
            {
                partSosi.startLifetime = 3.5f;
            }
         
      }
      else
      {
            partSosi.startLifetime = 3f;
            var vel = partSosi.velocityOverLifetime;
            vel.x = 0f;
            vel.y = 0f;
      }

      //Сила
      if (Input.GetMouseButtonDown(1) && !player.jumpAct && !player.runAct)
      {
         player.forceAct = true;
      }

      if (Input.GetMouseButtonUp(1))
      {
         player.forceAct = false;
      }
   }


   private void GameInput()
   {

      if (Input.GetMouseButtonDown(0))
      {
      }


      //Смена силы
      if (Input.GetKeyDown(KeyCode.Q))
      {
         player.SwitchForce();
      }


      //Действие или лечение
      if (Input.GetKey(KeyCode.E))
      {
      }


      //Перезарядка кристалла
      if (Input.GetKeyDown(KeyCode.R))
      {
         player.ReloadForce();
      }


      //Первая способность костюма
      if (Input.GetKeyDown(KeyCode.LeftShift) && !player.forceAct && !player.jumpAct)
      {
         player.runAct = true;
      }

      if (Input.GetKeyUp(KeyCode.LeftShift))
      {
         player.runAct = false;
      }

      //Вторая способность костюма
      if (Input.GetKeyDown(KeyCode.Space) && !player.forceAct && player.jumpCD<=0)
      {
         player.jumpAct = true;
      }


      //Сохранение
      if (Input.GetKey(KeyCode.F))
      {
         player.Save();
      }

      //Пейджер
      if (Input.GetKeyDown(KeyCode.Tab))
      {

      }

      //Меню
      if (Input.GetKeyDown(KeyCode.Escape))
      {

      }
   } 

   private void MenuInput()
   {

   }

}
