using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager: MonoBehaviour
{
   private InputManager inputMan;
   private bool cutSceneAct = true;
   private byte index =0;

    void Start()
    {
      inputMan = GameObject.Find("GameManager").GetComponent<InputManager>();
    }


    void Update()
    {
        
    }

   public void StartGame()
   {
      if (cutSceneAct)
      {
         
         cutSceneAct = false;
         StartCoroutine(endScene(index));
         index++;
      }
   } 

   IEnumerator endScene(byte index)
   {

      yield return new WaitForSeconds(2);

   }
}
