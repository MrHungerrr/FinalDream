using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
   private CameraController camControl;
   public GameObject startText;
   private InputManager inputMan;
   private Animator anim;
   private EnemyHelperAI eHelpAI;
   private bool cutSceneAct = true;
   private byte index = 0;
   public Collider first;
   public Collider second;

   void Awake()
   {
      camControl = GameObject.Find("Main Camera").GetComponent<CameraController>();
      anim = GetComponent<Animator>();
      eHelpAI = GameObject.Find("EnemyHelper").GetComponent<EnemyHelperAI>();
      inputMan = GameObject.Find("GameManager").GetComponent<InputManager>();
      anim.enabled = false;

   }

   void Start()
   {
      inputMan.cutScene = true;
      camControl.SwitchCamera();
      StartCoroutine(DemoStart());
   }



   public void CutSceneEnd()
   {
      if (cutSceneAct)
      {
         cutSceneAct = false;
         anim.enabled = true;
         anim.SetTrigger("StartCS0");
      }
   }

   private void EndCS0()
   {
      eHelpAI.night = true;
      eHelpAI.Night();
      inputMan.cutScene = false;
      camControl.SwitchCamera();
   }

   IEnumerator DemoStart()
   {
      while(cutSceneAct)
      {
         startText.SetActive(true);
         yield return new WaitForSeconds(1.5f);
         startText.SetActive(false);
         yield return new WaitForSeconds(1.5f);
      }
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.tag == "Player")
      {
         if (first.enabled == true)
         {
            first.enabled = false;
            eHelpAI.night = false;
            eHelpAI.Night();
         }
         else
         {
            second.enabled = false;
            eHelpAI.night = true;
            eHelpAI.Night();
            eHelpAI.orblessAI[0].Kill(GameObject.FindGameObjectWithTag("Player"));
         }
      }
   }

}
