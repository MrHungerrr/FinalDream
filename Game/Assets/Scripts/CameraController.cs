using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

   public GameObject cameraGame;
   public GameObject cameraCutScene;
   public InputManager inputMan;

   [ContextMenu("AutoFill")]
   public void Fill()
   {
      inputMan = GameObject.Find("GameManager").GetComponent<InputManager>();
   }

   void Start ()
   {
      inputMan = GameObject.Find("GameManager").GetComponent<InputManager>();
   }

   public void SwitchCamera()
   {
      if(inputMan.cutScene)
      {
         cameraGame.SetActive(false);
         cameraCutScene.SetActive(true);
      }
      else
      {
         cameraGame.SetActive(true);
         cameraCutScene.SetActive(false);
      }
   }
}
