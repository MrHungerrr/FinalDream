using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowingText: MonoBehaviour
{

   public float delay;
   private bool act;
   private bool destroy;
   public Renderer[] tips;
   private Color matColor;
   private float fade;
   public  PlayerScript pScript;
   public bool justDelay;
   public bool colExit;
   public bool keyWASD;
   public bool leftMouse;
   public bool rightMouse;
   public bool keyQ;
   public bool keyR;
   public bool keyE;
   public bool keyZ;
   public bool keyCTRL;
   public bool keySpace;
   public bool keyShift;


   [ContextMenu("AutoFill")]
   private void Fill()
   {
      pScript = GameObject.Find("Suit").GetComponent<PlayerScript>();
   }



   private void Start()
   {
      destroy = false;
      matColor = tips[0].material.color;
      fade = 0;
      for(int i = 0; i<tips.Length; i++)
         tips[i].material.SetColor("_Color", matColor * fade);
   }



   private void OnTriggerEnter(Collider obj)
   {
      if (obj.tag == "Player")
      {
         act = true;
      }
   }

   private void OnTriggerExit(Collider obj)
   {
      if (obj.tag == "Player")
      {
         if(colExit)
            delay = 22;
      }
   }


   void Update()
   {
      if (act)
      {
         if (justDelay) //Надпись появляется сразу. Исчезает после пары секунд.
         {
            if (delay > 0)
            {
               if (fade < 1)
               {
                  fade = Mathf.Lerp(fade, 1.1f, 0.03f);
                  for (int i = 0; i < tips.Length; i++)
                     tips[i].material.SetColor("_Color", matColor * fade);
               }
               else
               {
                  delay -= Time.deltaTime;
               }
            }
            else
            {
               StartCoroutine(DestroyTip());
            }
         }
         else if (colExit) //Надпись появляется сразу. Исчезает после выхода из кроайдера.
         {
            if (!destroy)
               if (delay != 22)
               {
                  ShowTip();
               }
               else
               {
                  destroy = true;
                  StartCoroutine(DestroyTip());
               }
         }
         else if (keyWASD) //Появляется через какое-то время. Исчезает как игрок начинает двигаться.
         {
            if (!destroy)
               if (!Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.S) && !Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.D))
               {
                  ShowTip();
               }
               else
               {
                  destroy = true;
                  StartCoroutine(DestroyTip());
               }
         }
         else if (rightMouse)  //Появляется через какое-то время. Исчезает как игрок начинает использовать силу.
         {
            if (!destroy)
               if (!pScript.forcePrep)
               {
                  ShowTip();
               }
               else
               {
                  destroy = true;
                  StartCoroutine(DestroyTip());
               }
         }
         else if (leftMouse) //Появляется через какое-то время. Исчезает как игрок начинает выпускать пламя.
         {
            if (!destroy)
               if (!pScript.forceAct)
               {
                  ShowTip();
               }
               else
               {
                  destroy = true;
                  StartCoroutine(DestroyTip());
               }
         }
         else if (keyQ) //Появляется через какое-то время. Исчезает как игрок сменяет магию.
         {
            if (!destroy)
               if (!pScript.forceSwitch)
               {
                  ShowTip();
               }
               else
               {
                  destroy = true;
                  StartCoroutine(DestroyTip());
               }
         }
         else if (keyE) //Появляется через какое-то время. Исчезает как игрок действует с окружением.
         {
            if (!destroy)
               if (!pScript.doing)
               {
                  ShowTip();
               }
               else
               {
                  destroy = true;
                  StartCoroutine(DestroyTip());
               }
         }
         else if (keyR) //Появляется через какое-то время. Исчезает как игрок меняет кристал.
         {
            if (!destroy)
               if (!pScript.doing)
               {
                  ShowTip();
               }
               else
               {
                  destroy = true;
                  StartCoroutine(DestroyTip());
               }
         }
         else if (keyShift) //Появляется через какое-то время. Исчезает как игрок начинает бежать.
         {
            if (!destroy)
               if (((!Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.S) && !Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.D)) || !pScript.runAct))
               {
                  ShowTip();
               }
               else
               {
                  destroy = true;
                  StartCoroutine(DestroyTip());
               }
         }
         else if (keySpace) //Появляется через какое-то время. Исчезает как прыгнет игрок.
         {
            if (!destroy)
               if (!pScript.jumpAct)
               {
                  ShowTip();
               }
               else
               {
                  destroy = true;
                  StartCoroutine(DestroyTip());
               }
         }
      }
   }




   private void ShowTip()
   {
      if (fade < 1)
      {
         if (delay > 0)
         {
            delay -= Time.deltaTime;
         }
         else
         {
            fade = Mathf.Lerp(fade, 1.1f, 0.03f);
            for (int i = 0; i < tips.Length; i++)
               tips[i].material.SetColor("_Color", matColor * fade);
         }
      }
   }


   IEnumerator DestroyTip()
   {
      yield return new WaitForSeconds(1f);
      for (int i = 0; i < tips.Length; i++)
         tips[i].material.SetColor("_Color", matColor * 0);
      yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
      for (int i = 0; i < tips.Length; i++)
         tips[i].material.SetColor("_Color", matColor * fade);
      yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
      for (int i = 0; i < tips.Length; i++)
         tips[i].material.SetColor("_Color", matColor * 0);
      yield return new WaitForSeconds(Random.Range(0.1f, 1f));
      for (int i = 0; i < tips.Length; i++)
         tips[i].material.SetColor("_Color", matColor * fade);
      yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
      for (int i = 0; i < tips.Length; i++)
         tips[i].material.SetColor("_Color", matColor * 0);
      yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
      for (int i = 0; i < tips.Length; i++)
         tips[i].material.SetColor("_Color", matColor * fade);
      yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
      for (int i = 0; i < tips.Length; i++)
         tips[i].material.SetColor("_Color", matColor * 0);
      Destroy(this.gameObject);
   }
}
